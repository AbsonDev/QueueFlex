# 🚀 Deploy e Infraestrutura

## Visão Geral

Este documento descreve as estratégias de deploy, configurações de infraestrutura e melhores práticas para colocar o Queue Management System em produção.

## 🐳 Docker

### Dockerfile para API

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QueueManagement.Api/QueueManagement.Api.csproj", "QueueManagement.Api/"]
COPY ["Domain/QueueManagement.Domain.csproj", "Domain/"]
COPY ["Infrastructure/QueueManagement.Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "QueueManagement.Api/QueueManagement.Api.csproj"
COPY . .
WORKDIR "/src/QueueManagement.Api"
RUN dotnet build "QueueManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QueueManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QueueManagement.Api.dll"]
```

### Dockerfile para Frontend

```dockerfile
# Dockerfile.frontend
FROM node:18-alpine AS build
WORKDIR /app
COPY queuemanagement-admin/package*.json ./
RUN npm ci
COPY queuemanagement-admin/ .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Docker Compose

```yaml
# docker-compose.yml
version: '3.8'

services:
  postgres:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: queue_management
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    networks:
      - queue_network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - queue_network
    command: redis-server --appendonly yes
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build:
      context: .
      dockerfile: Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Host=postgres;Database=queue_management;Username=postgres;Password=${DB_PASSWORD}"
      ConnectionStrings__Redis: "redis:6379"
      Jwt__SecretKey: ${JWT_SECRET}
    ports:
      - "5000:80"
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy
    networks:
      - queue_network
    restart: unless-stopped

  frontend:
    build:
      context: .
      dockerfile: Dockerfile.frontend
    ports:
      - "3000:80"
    depends_on:
      - api
    networks:
      - queue_network
    restart: unless-stopped

volumes:
  postgres_data:
  redis_data:

networks:
  queue_network:
    driver: bridge
```

## ☸️ Kubernetes

### Deployment da API

```yaml
# k8s/api-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: queue-api
  labels:
    app: queue-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: queue-api
  template:
    metadata:
      labels:
        app: queue-api
    spec:
      containers:
      - name: api
        image: queue-management/api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: queue-secrets
              key: db-connection
        - name: Jwt__SecretKey
          valueFrom:
            secretKeyRef:
              name: queue-secrets
              key: jwt-secret
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: queue-api-service
spec:
  selector:
    app: queue-api
  ports:
    - protocol: TCP
      port: 80
      targetPort: 80
  type: LoadBalancer
```

### ConfigMap e Secrets

```yaml
# k8s/configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: queue-config
data:
  appsettings.Production.json: |
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning"
        }
      },
      "AllowedHosts": "*",
      "Cors": {
        "AllowedOrigins": ["https://app.queuemanagement.com"]
      }
    }
---
# k8s/secrets.yaml
apiVersion: v1
kind: Secret
metadata:
  name: queue-secrets
type: Opaque
stringData:
  db-connection: "Host=postgres;Database=queue_management;Username=postgres;Password=SecurePassword123"
  jwt-secret: "your-super-secret-key-with-at-least-256-bits"
  redis-connection: "redis:6379,password=RedisPassword123"
```

### Horizontal Pod Autoscaler

```yaml
# k8s/hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: queue-api-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: queue-api
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

### Ingress

```yaml
# k8s/ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: queue-ingress
  annotations:
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
    nginx.ingress.kubernetes.io/rate-limit: "100"
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
spec:
  tls:
  - hosts:
    - api.queuemanagement.com
    - app.queuemanagement.com
    secretName: queue-tls
  rules:
  - host: api.queuemanagement.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: queue-api-service
            port:
              number: 80
  - host: app.queuemanagement.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: queue-frontend-service
            port:
              number: 80
```

## ☁️ Cloud Providers

### AWS

#### Infrastructure as Code (Terraform)

```hcl
# terraform/aws/main.tf
provider "aws" {
  region = var.aws_region
}

# VPC
resource "aws_vpc" "queue_vpc" {
  cidr_block           = "10.0.0.0/16"
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = {
    Name = "queue-management-vpc"
  }
}

# RDS PostgreSQL
resource "aws_db_instance" "queue_db" {
  identifier     = "queue-management-db"
  engine         = "postgres"
  engine_version = "15.3"
  instance_class = "db.t3.medium"
  
  allocated_storage     = 100
  storage_type         = "gp3"
  storage_encrypted    = true
  
  db_name  = "queue_management"
  username = var.db_username
  password = var.db_password
  
  vpc_security_group_ids = [aws_security_group.db_sg.id]
  db_subnet_group_name   = aws_db_subnet_group.queue_subnet.name
  
  backup_retention_period = 30
  backup_window          = "03:00-04:00"
  maintenance_window     = "sun:04:00-sun:05:00"
  
  skip_final_snapshot = false
  final_snapshot_identifier = "queue-db-final-snapshot-${timestamp()}"
  
  tags = {
    Name = "queue-management-database"
  }
}

# ElastiCache Redis
resource "aws_elasticache_cluster" "queue_cache" {
  cluster_id           = "queue-cache"
  engine              = "redis"
  node_type           = "cache.t3.micro"
  num_cache_nodes     = 1
  parameter_group_name = "default.redis7"
  port                = 6379
  
  subnet_group_name = aws_elasticache_subnet_group.cache_subnet.name
  security_group_ids = [aws_security_group.cache_sg.id]
  
  tags = {
    Name = "queue-management-cache"
  }
}

# ECS Fargate
resource "aws_ecs_cluster" "queue_cluster" {
  name = "queue-management-cluster"
  
  setting {
    name  = "containerInsights"
    value = "enabled"
  }
}

resource "aws_ecs_task_definition" "queue_api" {
  family                   = "queue-api"
  network_mode            = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                     = "512"
  memory                  = "1024"
  execution_role_arn      = aws_iam_role.ecs_execution_role.arn
  task_role_arn           = aws_iam_role.ecs_task_role.arn
  
  container_definitions = jsonencode([
    {
      name  = "api"
      image = "${aws_ecr_repository.queue_api.repository_url}:latest"
      
      portMappings = [
        {
          containerPort = 80
          protocol      = "tcp"
        }
      ]
      
      environment = [
        {
          name  = "ASPNETCORE_ENVIRONMENT"
          value = "Production"
        }
      ]
      
      secrets = [
        {
          name      = "ConnectionStrings__DefaultConnection"
          valueFrom = aws_secretsmanager_secret.db_connection.arn
        },
        {
          name      = "Jwt__SecretKey"
          valueFrom = aws_secretsmanager_secret.jwt_secret.arn
        }
      ]
      
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          "awslogs-group"         = "/ecs/queue-api"
          "awslogs-region"        = var.aws_region
          "awslogs-stream-prefix" = "ecs"
        }
      }
    }
  ])
}

# Application Load Balancer
resource "aws_lb" "queue_alb" {
  name               = "queue-management-alb"
  internal           = false
  load_balancer_type = "application"
  security_groups    = [aws_security_group.alb_sg.id]
  subnets           = aws_subnet.public.*.id
  
  enable_deletion_protection = true
  enable_http2              = true
  
  tags = {
    Name = "queue-management-alb"
  }
}
```

### Azure

#### ARM Template

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "appName": {
      "type": "string",
      "defaultValue": "queue-management"
    }
  },
  "resources": [
    {
      "type": "Microsoft.Web/serverfarms",
      "apiVersion": "2021-02-01",
      "name": "[concat(parameters('appName'), '-plan')]",
      "location": "[resourceGroup().location]",
      "sku": {
        "name": "P1v2",
        "tier": "PremiumV2",
        "size": "P1v2",
        "family": "Pv2",
        "capacity": 1
      },
      "kind": "linux",
      "properties": {
        "reserved": true
      }
    },
    {
      "type": "Microsoft.Web/sites",
      "apiVersion": "2021-02-01",
      "name": "[parameters('appName')]",
      "location": "[resourceGroup().location]",
      "dependsOn": [
        "[resourceId('Microsoft.Web/serverfarms', concat(parameters('appName'), '-plan'))]"
      ],
      "properties": {
        "serverFarmId": "[resourceId('Microsoft.Web/serverfarms', concat(parameters('appName'), '-plan'))]",
        "siteConfig": {
          "linuxFxVersion": "DOTNETCORE|8.0",
          "appSettings": [
            {
              "name": "ASPNETCORE_ENVIRONMENT",
              "value": "Production"
            }
          ]
        }
      }
    },
    {
      "type": "Microsoft.DBforPostgreSQL/servers",
      "apiVersion": "2017-12-01",
      "name": "[concat(parameters('appName'), '-db')]",
      "location": "[resourceGroup().location]",
      "sku": {
        "name": "GP_Gen5_2",
        "tier": "GeneralPurpose",
        "family": "Gen5",
        "capacity": 2
      },
      "properties": {
        "version": "11",
        "sslEnforcement": "Enabled",
        "storageProfile": {
          "storageMB": 102400,
          "backupRetentionDays": 30,
          "geoRedundantBackup": "Enabled"
        }
      }
    }
  ]
}
```

### Google Cloud Platform

```yaml
# gcp/app.yaml
runtime: aspnetcore
env: flex

automatic_scaling:
  min_num_instances: 2
  max_num_instances: 10
  cool_down_period_sec: 120
  cpu_utilization:
    target_utilization: 0.7

resources:
  cpu: 2
  memory_gb: 4
  disk_size_gb: 20

env_variables:
  ASPNETCORE_ENVIRONMENT: "Production"

beta_settings:
  cloud_sql_instances: "PROJECT_ID:REGION:INSTANCE_NAME"
```

## 📊 Monitoramento

### Prometheus + Grafana

```yaml
# docker-compose.monitoring.yml
version: '3.8'

services:
  prometheus:
    image: prom/prometheus:latest
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus_data:/prometheus
    command:
      - '--config.file=/etc/prometheus/prometheus.yml'
      - '--storage.tsdb.path=/prometheus'
    ports:
      - "9090:9090"
    networks:
      - monitoring

  grafana:
    image: grafana/grafana:latest
    volumes:
      - grafana_data:/var/lib/grafana
      - ./grafana/dashboards:/etc/grafana/provisioning/dashboards
      - ./grafana/datasources:/etc/grafana/provisioning/datasources
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin
      - GF_INSTALL_PLUGINS=grafana-piechart-panel
    ports:
      - "3001:3000"
    networks:
      - monitoring

  node-exporter:
    image: prom/node-exporter:latest
    ports:
      - "9100:9100"
    networks:
      - monitoring

  cadvisor:
    image: gcr.io/cadvisor/cadvisor:latest
    volumes:
      - /:/rootfs:ro
      - /var/run:/var/run:ro
      - /sys:/sys:ro
      - /var/lib/docker/:/var/lib/docker:ro
    ports:
      - "8080:8080"
    networks:
      - monitoring

volumes:
  prometheus_data:
  grafana_data:

networks:
  monitoring:
    driver: bridge
```

### Configuração Prometheus

```yaml
# prometheus.yml
global:
  scrape_interval: 15s
  evaluation_interval: 15s

scrape_configs:
  - job_name: 'queue-api'
    static_configs:
      - targets: ['api:80']
    metrics_path: '/metrics'

  - job_name: 'node'
    static_configs:
      - targets: ['node-exporter:9100']

  - job_name: 'cadvisor'
    static_configs:
      - targets: ['cadvisor:8080']

  - job_name: 'postgres'
    static_configs:
      - targets: ['postgres-exporter:9187']
```

## 🔄 CI/CD

### GitHub Actions

```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [main]
  workflow_dispatch:

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Test
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      
      - name: Upload coverage
        uses: codecov/codecov-action@v3

  build-and-push:
    needs: test
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Log in to Container Registry
        uses: docker/login-action@v2
        with:
          registry: ${{ env.REGISTRY }}
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}
      
      - name: Build and push Docker image
        uses: docker/build-push-action@v4
        with:
          context: .
          push: true
          tags: |
            ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:latest
            ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}

  deploy:
    needs: build-and-push
    runs-on: ubuntu-latest
    environment: production
    
    steps:
      - name: Deploy to Kubernetes
        uses: azure/k8s-deploy@v4
        with:
          manifests: |
            k8s/deployment.yaml
            k8s/service.yaml
          images: |
            ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}:${{ github.sha }}
```

## 🔐 Backup e Recovery

### Backup Automático PostgreSQL

```bash
#!/bin/bash
# backup.sh

DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups"
DB_NAME="queue_management"
S3_BUCKET="queue-backups"

# Backup local
pg_dump -h localhost -U postgres -d $DB_NAME | gzip > $BACKUP_DIR/backup_$DATE.sql.gz

# Upload para S3
aws s3 cp $BACKUP_DIR/backup_$DATE.sql.gz s3://$S3_BUCKET/

# Limpar backups antigos (manter últimos 30 dias)
find $BACKUP_DIR -name "backup_*.sql.gz" -mtime +30 -delete

# Verificar backup
if [ $? -eq 0 ]; then
    echo "Backup completed successfully: backup_$DATE.sql.gz"
else
    echo "Backup failed!"
    exit 1
fi
```

### Restore Procedure

```bash
#!/bin/bash
# restore.sh

BACKUP_FILE=$1
DB_NAME="queue_management"

if [ -z "$BACKUP_FILE" ]; then
    echo "Usage: ./restore.sh backup_file.sql.gz"
    exit 1
fi

# Download from S3 if needed
if [[ $BACKUP_FILE == s3://* ]]; then
    aws s3 cp $BACKUP_FILE /tmp/restore.sql.gz
    BACKUP_FILE="/tmp/restore.sql.gz"
fi

# Restore database
gunzip -c $BACKUP_FILE | psql -h localhost -U postgres -d $DB_NAME

if [ $? -eq 0 ]; then
    echo "Restore completed successfully"
else
    echo "Restore failed!"
    exit 1
fi
```

## 📈 Scaling Strategy

### Vertical Scaling

```yaml
# Aumentar recursos conforme necessário
resources:
  requests:
    memory: "512Mi"  # -> 1Gi -> 2Gi
    cpu: "500m"       # -> 1000m -> 2000m
  limits:
    memory: "1Gi"     # -> 2Gi -> 4Gi
    cpu: "1000m"      # -> 2000m -> 4000m
```

### Horizontal Scaling

```yaml
# HPA baseado em métricas
metrics:
  - type: Pods
    pods:
      metric:
        name: http_requests_per_second
      target:
        type: AverageValue
        averageValue: "1000"
```

### Database Scaling

```sql
-- Particionamento por tenant para grandes volumes
CREATE TABLE tickets_2024 PARTITION OF tickets
FOR VALUES FROM ('2024-01-01') TO ('2025-01-01');

-- Índices parciais para performance
CREATE INDEX idx_active_tickets ON tickets (status, queue_id)
WHERE is_deleted = false AND status IN ('Waiting', 'Called');
```

## 🔍 Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString,
        name: "database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "postgres" })
    .AddRedis(
        redisConnectionString,
        name: "redis",
        failureStatus: HealthStatus.Degraded)
    .AddUrlGroup(
        new Uri("https://api.external.com/health"),
        name: "external-api",
        failureStatus: HealthStatus.Degraded);

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
```

## 📋 Deployment Checklist

### Pre-deployment

- [ ] Código revisado e aprovado
- [ ] Testes passando (unit, integration, e2e)
- [ ] Build bem-sucedido
- [ ] Migrations testadas
- [ ] Configurações de produção validadas
- [ ] Secrets configurados
- [ ] Backup do banco realizado

### Deployment

- [ ] Deploy em staging primeiro
- [ ] Smoke tests em staging
- [ ] Deploy gradual (canary/blue-green)
- [ ] Monitoramento ativo
- [ ] Logs sendo coletados

### Post-deployment

- [ ] Health checks verdes
- [ ] Métricas normais
- [ ] Sem erros críticos nos logs
- [ ] Performance adequada
- [ ] Notificar stakeholders
- [ ] Documentar mudanças

## 🚨 Rollback Strategy

```bash
#!/bin/bash
# rollback.sh

PREVIOUS_VERSION=$1

# Rollback Kubernetes deployment
kubectl rollout undo deployment/queue-api

# Ou especificar versão
kubectl rollout undo deployment/queue-api --to-revision=$PREVIOUS_VERSION

# Verificar status
kubectl rollout status deployment/queue-api

# Rollback database se necessário
./restore.sh s3://queue-backups/backup_before_deploy.sql.gz
```

---

📝 **Última atualização**: Dezembro 2024
🚀 **Versão**: 1.0.0