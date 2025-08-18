using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace QueueManagement.ValidationTests
{
    public class SimpleValidation
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private string? _token;
        private string? _tenantId;
        private string? _unitId;
        private readonly Dictionary<string, string> _serviceIds = new();
        private readonly Dictionary<string, string> _queueIds = new();
        private readonly List<string> _ticketIds = new();

        public SimpleValidation(string baseUrl = "http://localhost:5000")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        public async Task<bool> RunValidation()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("QUEUE MANAGEMENT API - VALIDAÇÃO COMPLETA");
            Console.WriteLine("========================================\n");

            var results = new List<(string Test, bool Success, string Message)>();
            
            try
            {
                // 1. Setup & Configuração
                Console.WriteLine("📋 1. SETUP & CONFIGURAÇÃO");
                Console.WriteLine("--------------------------");
                
                var setupResult = await ValidateSetup();
                results.Add(("Setup & Configuração", setupResult.Success, setupResult.Message));
                
                if (!setupResult.Success)
                {
                    Console.WriteLine($"❌ Setup falhou: {setupResult.Message}");
                    return false;
                }

                // 2. Serviços & Filas
                Console.WriteLine("\n📋 2. SERVIÇOS & FILAS");
                Console.WriteLine("----------------------");
                
                var servicesResult = await ValidateServicesAndQueues();
                results.Add(("Serviços & Filas", servicesResult.Success, servicesResult.Message));

                // 3. Fluxo de Atendimento
                Console.WriteLine("\n📋 3. FLUXO DE ATENDIMENTO");
                Console.WriteLine("--------------------------");
                
                var flowResult = await ValidateAttendanceFlow();
                results.Add(("Fluxo de Atendimento", flowResult.Success, flowResult.Message));

                // 4. Business Rules
                Console.WriteLine("\n📋 4. BUSINESS RULES");
                Console.WriteLine("--------------------");
                
                var rulesResult = await ValidateBusinessRules();
                results.Add(("Business Rules", rulesResult.Success, rulesResult.Message));

                // 5. Performance
                Console.WriteLine("\n📋 5. PERFORMANCE");
                Console.WriteLine("-----------------");
                
                var perfResult = await ValidatePerformance();
                results.Add(("Performance", perfResult.Success, perfResult.Message));

                // Relatório Final
                Console.WriteLine("\n========================================");
                Console.WriteLine("RELATÓRIO FINAL");
                Console.WriteLine("========================================\n");
                
                var totalTests = results.Count;
                var passedTests = results.Count(r => r.Success);
                var failedTests = totalTests - passedTests;
                var successRate = (double)passedTests / totalTests * 100;

                Console.WriteLine($"Total de Testes: {totalTests}");
                Console.WriteLine($"✅ Aprovados: {passedTests}");
                Console.WriteLine($"❌ Falhados: {failedTests}");
                Console.WriteLine($"📊 Taxa de Sucesso: {successRate:F1}%\n");

                Console.WriteLine("Detalhes:");
                Console.WriteLine("---------");
                foreach (var (test, success, message) in results)
                {
                    var icon = success ? "✅" : "❌";
                    Console.WriteLine($"{icon} {test}: {message}");
                }

                Console.WriteLine("\n========================================");
                if (successRate >= 90)
                {
                    Console.WriteLine("✅ SISTEMA APROVADO - Pronto para produção!");
                }
                else if (successRate >= 70)
                {
                    Console.WriteLine("⚠️  SISTEMA PARCIALMENTE APROVADO - Correções necessárias");
                }
                else
                {
                    Console.WriteLine("❌ SISTEMA REPROVADO - Múltiplos problemas críticos");
                }
                Console.WriteLine("========================================");

                return successRate >= 70;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ ERRO FATAL: {ex.Message}");
                return false;
            }
        }

        private async Task<(bool Success, string Message)> ValidateSetup()
        {
            try
            {
                // 1.1 Criar Tenant
                Console.WriteLine("  1.1 Criando tenant...");
                var registerData = new
                {
                    tenantName = $"Clínica São Paulo {DateTime.Now:yyyyMMddHHmmss}",
                    subdomain = $"clinica-sp-{DateTime.Now:yyyyMMddHHmmss}",
                    adminName = "Dr. Carlos Silva",
                    adminEmail = $"admin{DateTime.Now:yyyyMMddHHmmss}@clinicasp.com",
                    password = "Admin123!"
                };

                var response = await PostAsync("/api/v1/auth/register", registerData);
                if (response == null || !response.ContainsKey("token"))
                {
                    return (false, "Falha ao criar tenant");
                }

                _token = response["token"]?.ToString();
                _tenantId = response["tenantId"]?.ToString();
                Console.WriteLine($"    ✓ Tenant criado: {_tenantId}");

                // 1.2 Criar Unidade
                Console.WriteLine("  1.2 Criando unidade...");
                var unitData = new
                {
                    name = "Unidade Centro",
                    code = $"CENTRO{DateTime.Now:HHmmss}",
                    address = new
                    {
                        street = "Rua das Flores",
                        number = "123",
                        city = "São Paulo",
                        state = "SP",
                        zipCode = "01234-567",
                        country = "Brasil"
                    }
                };

                var unitResponse = await PostAsync("/api/v1/units", unitData, true);
                if (unitResponse == null || !unitResponse.ContainsKey("id"))
                {
                    return (false, "Falha ao criar unidade");
                }

                _unitId = unitResponse["id"]?.ToString();
                Console.WriteLine($"    ✓ Unidade criada: {unitResponse["name"]}");

                return (true, "Setup concluído com sucesso");
            }
            catch (Exception ex)
            {
                return (false, $"Erro no setup: {ex.Message}");
            }
        }

        private async Task<(bool Success, string Message)> ValidateServicesAndQueues()
        {
            try
            {
                // 2.1 Criar Serviços
                Console.WriteLine("  2.1 Criando serviços...");
                var services = new[]
                {
                    new { name = "Consulta Geral", code = $"CONS{DateTime.Now:HHmmss}", estimatedDurationMinutes = 30 },
                    new { name = "Exames", code = $"EXAM{DateTime.Now:HHmmss}", estimatedDurationMinutes = 15 },
                    new { name = "Retorno", code = $"RET{DateTime.Now:HHmmss}", estimatedDurationMinutes = 15 }
                };

                foreach (var service in services)
                {
                    var response = await PostAsync("/api/v1/services", service, true);
                    if (response != null && response.ContainsKey("id"))
                    {
                        _serviceIds[service.name] = response["id"]?.ToString() ?? "";
                        Console.WriteLine($"    ✓ Serviço criado: {service.name}");
                    }
                }

                // 2.2 Criar Filas
                Console.WriteLine("  2.2 Criando filas...");
                var queues = new[]
                {
                    new { name = "Atendimento Geral", code = $"A{DateTime.Now:HHmmss}", displayName = "Fila A", maxCapacity = 50 },
                    new { name = "Exames", code = $"B{DateTime.Now:HHmmss}", displayName = "Fila B", maxCapacity = 30 },
                    new { name = "Preferencial", code = $"P{DateTime.Now:HHmmss}", displayName = "Fila P", maxCapacity = 20 }
                };

                foreach (var queue in queues)
                {
                    var response = await PostAsync($"/api/v1/units/{_unitId}/queues", queue, true);
                    if (response != null && response.ContainsKey("id"))
                    {
                        _queueIds[queue.name] = response["id"]?.ToString() ?? "";
                        Console.WriteLine($"    ✓ Fila criada: {queue.name}");
                    }
                }

                return (true, "Serviços e filas criados com sucesso");
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao criar serviços/filas: {ex.Message}");
            }
        }

        private async Task<(bool Success, string Message)> ValidateAttendanceFlow()
        {
            try
            {
                // 3.1 Criar Tickets
                Console.WriteLine("  3.1 Criando 10 tickets...");
                var ticketCount = 0;
                
                for (int i = 1; i <= 10; i++)
                {
                    var queueId = _queueIds.Values.First();
                    var serviceId = _serviceIds.Values.First();
                    
                    var ticketData = new
                    {
                        queueId = queueId,
                        serviceId = serviceId,
                        customerName = $"Paciente {i}",
                        priority = i % 4 // Varia prioridades
                    };

                    var response = await PostAsync("/api/v1/tickets", ticketData, true);
                    if (response != null && response.ContainsKey("id"))
                    {
                        _ticketIds.Add(response["id"]?.ToString() ?? "");
                        ticketCount++;
                    }
                }

                Console.WriteLine($"    ✓ {ticketCount} tickets criados");

                // 3.2 Verificar Status da Fila
                Console.WriteLine("  3.2 Verificando status das filas...");
                foreach (var (name, queueId) in _queueIds)
                {
                    var status = await GetAsync($"/api/v1/queues/{queueId}/status", true);
                    if (status != null)
                    {
                        var waiting = status["waitingCount"]?.ToString() ?? "0";
                        Console.WriteLine($"    ✓ Fila {name}: {waiting} aguardando");
                    }
                }

                return (true, $"Fluxo validado com {ticketCount} tickets");
            }
            catch (Exception ex)
            {
                return (false, $"Erro no fluxo: {ex.Message}");
            }
        }

        private async Task<(bool Success, string Message)> ValidateBusinessRules()
        {
            try
            {
                Console.WriteLine("  4.1 Testando capacidade de fila...");
                
                // Criar fila com capacidade limitada
                var testQueue = new
                {
                    name = "Fila Teste",
                    code = $"TEST{DateTime.Now:HHmmss}",
                    displayName = "Teste",
                    maxCapacity = 3
                };

                var queueResponse = await PostAsync($"/api/v1/units/{_unitId}/queues", testQueue, true);
                if (queueResponse == null || !queueResponse.ContainsKey("id"))
                {
                    return (false, "Falha ao criar fila de teste");
                }

                var testQueueId = queueResponse["id"]?.ToString();
                var serviceId = _serviceIds.Values.First();

                // Tentar criar tickets além da capacidade
                var successCount = 0;
                var blockedCount = 0;

                for (int i = 1; i <= 5; i++)
                {
                    var ticketData = new
                    {
                        queueId = testQueueId,
                        serviceId = serviceId,
                        customerName = $"Teste {i}",
                        priority = 1
                    };

                    var response = await PostAsync("/api/v1/tickets", ticketData, true);
                    if (response != null && response.ContainsKey("id"))
                    {
                        successCount++;
                    }
                    else
                    {
                        blockedCount++;
                    }
                }

                Console.WriteLine($"    ✓ Capacidade respeitada: {successCount} criados, {blockedCount} bloqueados");

                return (true, "Business rules validadas");
            }
            catch (Exception ex)
            {
                return (false, $"Erro nas business rules: {ex.Message}");
            }
        }

        private async Task<(bool Success, string Message)> ValidatePerformance()
        {
            try
            {
                Console.WriteLine("  5.1 Testando performance (50 requisições)...");
                
                var queueId = _queueIds.Values.First();
                var stopwatch = Stopwatch.StartNew();
                var successCount = 0;

                var tasks = new List<Task<Dictionary<string, object>?>>();
                for (int i = 0; i < 50; i++)
                {
                    tasks.Add(GetAsync($"/api/v1/queues/{queueId}/status", true));
                }

                var results = await Task.WhenAll(tasks);
                successCount = results.Count(r => r != null);
                
                stopwatch.Stop();
                var avgMs = stopwatch.ElapsedMilliseconds / 50.0;

                Console.WriteLine($"    ✓ {successCount}/50 requisições bem-sucedidas");
                Console.WriteLine($"    ✓ Tempo médio: {avgMs:F2}ms");

                var performanceOk = avgMs < 200; // Critério: < 200ms por requisição
                
                return (performanceOk, $"Performance {(performanceOk ? "adequada" : "inadequada")}: {avgMs:F2}ms/req");
            }
            catch (Exception ex)
            {
                return (false, $"Erro no teste de performance: {ex.Message}");
            }
        }

        private async Task<Dictionary<string, object>?> PostAsync(string endpoint, object data, bool authenticated = false)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                if (authenticated && !string.IsNullOrEmpty(_token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                }

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<Dictionary<string, object>?> GetAsync(string endpoint, bool authenticated = false)
        {
            try
            {
                if (authenticated && !string.IsNullOrEmpty(_token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                }

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var validator = new SimpleValidation();
            var success = await validator.RunValidation();
            
            Environment.Exit(success ? 0 : 1);
        }
    }
}