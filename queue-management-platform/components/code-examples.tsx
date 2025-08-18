'use client'

import { useState } from 'react'
import { motion } from 'framer-motion'
import { Copy, Check } from 'lucide-react'

const languages = [
  { id: 'javascript', name: 'JavaScript', icon: '🟨' },
  { id: 'python', name: 'Python', icon: '🐍' },
  { id: 'curl', name: 'cURL', icon: '🔗' },
  { id: 'csharp', name: 'C#', icon: '🔷' },
  { id: 'java', name: 'Java', icon: '☕' },
]

const codeExamples = {
  javascript: `import { QueueAPI } from '@queue-api/sdk';

const api = new QueueAPI({
  apiKey: process.env.QUEUE_API_KEY,
  region: 'us-east-1'
});

// Create a queue
const queue = await api.queues.create({
  name: 'Customer Support',
  maxCapacity: 100,
  averageServiceTime: 5,
  priority: 'FIFO'
});

// Generate a ticket
const ticket = await api.tickets.generate({
  queueId: queue.id,
  customer: {
    name: 'John Doe',
    email: 'john@example.com',
    phone: '+1234567890'
  },
  metadata: {
    issue: 'Technical Support',
    priority: 'high'
  }
});

// Real-time updates via WebSocket
api.on('ticket.called', (data) => {
  console.log(\`Ticket \${data.number} is being served\`);
});`,
  python: `from queue_api import QueueAPI

api = QueueAPI(
    api_key=os.environ['QUEUE_API_KEY'],
    region='us-east-1'
)

# Create a queue
queue = api.queues.create(
    name='Customer Support',
    max_capacity=100,
    average_service_time=5,
    priority='FIFO'
)

# Generate a ticket
ticket = api.tickets.generate(
    queue_id=queue.id,
    customer={
        'name': 'John Doe',
        'email': 'john@example.com',
        'phone': '+1234567890'
    },
    metadata={
        'issue': 'Technical Support',
        'priority': 'high'
    }
)

# Real-time updates via WebSocket
@api.on('ticket.called')
def on_ticket_called(data):
    print(f"Ticket {data['number']} is being served")`,
  curl: `# Create a queue
curl -X POST https://api.queuemanagement.dev/v1/queues \\
  -H "Authorization: Bearer YOUR_API_KEY" \\
  -H "Content-Type: application/json" \\
  -d '{
    "name": "Customer Support",
    "maxCapacity": 100,
    "averageServiceTime": 5,
    "priority": "FIFO"
  }'

# Generate a ticket
curl -X POST https://api.queuemanagement.dev/v1/tickets \\
  -H "Authorization: Bearer YOUR_API_KEY" \\
  -H "Content-Type: application/json" \\
  -d '{
    "queueId": "queue_123",
    "customer": {
      "name": "John Doe",
      "email": "john@example.com",
      "phone": "+1234567890"
    },
    "metadata": {
      "issue": "Technical Support",
      "priority": "high"
    }
  }'`,
  csharp: `using QueueManagement.SDK;

var api = new QueueAPI(new QueueAPIConfig
{
    ApiKey = Environment.GetEnvironmentVariable("QUEUE_API_KEY"),
    Region = "us-east-1"
});

// Create a queue
var queue = await api.Queues.CreateAsync(new CreateQueueRequest
{
    Name = "Customer Support",
    MaxCapacity = 100,
    AverageServiceTime = 5,
    Priority = QueuePriority.FIFO
});

// Generate a ticket
var ticket = await api.Tickets.GenerateAsync(new GenerateTicketRequest
{
    QueueId = queue.Id,
    Customer = new Customer
    {
        Name = "John Doe",
        Email = "john@example.com",
        Phone = "+1234567890"
    },
    Metadata = new Dictionary<string, object>
    {
        ["issue"] = "Technical Support",
        ["priority"] = "high"
    }
});

// Real-time updates via WebSocket
api.OnTicketCalled += (sender, data) =>
{
    Console.WriteLine($"Ticket {data.Number} is being served");
};`,
  java: `import dev.queuemanagement.sdk.QueueAPI;
import dev.queuemanagement.sdk.models.*;

QueueAPI api = new QueueAPI.Builder()
    .apiKey(System.getenv("QUEUE_API_KEY"))
    .region("us-east-1")
    .build();

// Create a queue
Queue queue = api.queues().create(
    CreateQueueRequest.builder()
        .name("Customer Support")
        .maxCapacity(100)
        .averageServiceTime(5)
        .priority(QueuePriority.FIFO)
        .build()
);

// Generate a ticket
Ticket ticket = api.tickets().generate(
    GenerateTicketRequest.builder()
        .queueId(queue.getId())
        .customer(Customer.builder()
            .name("John Doe")
            .email("john@example.com")
            .phone("+1234567890")
            .build())
        .metadata(Map.of(
            "issue", "Technical Support",
            "priority", "high"
        ))
        .build()
);

// Real-time updates via WebSocket
api.onTicketCalled(data -> {
    System.out.println("Ticket " + data.getNumber() + " is being served");
});`,
}

export function CodeExamples() {
  const [selectedLanguage, setSelectedLanguage] = useState('javascript')
  const [copied, setCopied] = useState(false)

  const copyToClipboard = () => {
    navigator.clipboard.writeText(codeExamples[selectedLanguage as keyof typeof codeExamples])
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  return (
    <section className="py-24 bg-gray-50 dark:bg-gray-900/50">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <motion.h2
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl dark:text-white"
          >
            Start Coding in Your Favorite Language
          </motion.h2>
          <motion.p
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.1 }}
            className="mt-4 text-lg text-gray-600 dark:text-gray-300"
          >
            Official SDKs and comprehensive documentation for seamless integration
          </motion.p>
        </div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          transition={{ delay: 0.2 }}
          className="mt-12"
        >
          <div className="rounded-2xl bg-white shadow-xl dark:bg-gray-800">
            {/* Language tabs */}
            <div className="flex overflow-x-auto border-b border-gray-200 dark:border-gray-700">
              {languages.map((lang) => (
                <button
                  key={lang.id}
                  onClick={() => setSelectedLanguage(lang.id)}
                  className={`flex items-center space-x-2 px-6 py-4 text-sm font-medium transition-colors ${
                    selectedLanguage === lang.id
                      ? 'border-b-2 border-blue-600 text-blue-600 dark:text-blue-400'
                      : 'text-gray-600 hover:text-gray-900 dark:text-gray-400 dark:hover:text-gray-200'
                  }`}
                >
                  <span className="text-lg">{lang.icon}</span>
                  <span>{lang.name}</span>
                </button>
              ))}
            </div>

            {/* Code display */}
            <div className="relative">
              <button
                onClick={copyToClipboard}
                className="absolute right-4 top-4 rounded-lg bg-gray-800 p-2 text-gray-400 hover:bg-gray-700 hover:text-white transition-colors"
              >
                {copied ? (
                  <Check className="h-5 w-5" />
                ) : (
                  <Copy className="h-5 w-5" />
                )}
              </button>
              
              <div className="p-6">
                <pre className="overflow-x-auto rounded-lg bg-gray-900 p-6">
                  <code className="text-sm text-gray-300">
                    {codeExamples[selectedLanguage as keyof typeof codeExamples]}
                  </code>
                </pre>
              </div>
            </div>
          </div>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          transition={{ delay: 0.3 }}
          className="mt-8 flex justify-center space-x-4"
        >
          <a
            href="/docs"
            className="inline-flex items-center text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300"
          >
            View Full Documentation →
          </a>
          <a
            href="/playground"
            className="inline-flex items-center text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300"
          >
            Try in Playground →
          </a>
        </motion.div>
      </div>
    </section>
  )
}