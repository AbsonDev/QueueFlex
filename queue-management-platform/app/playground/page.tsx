'use client'

import { useState } from 'react'
import { Navbar } from '@/components/navbar'
import { Footer } from '@/components/footer'
import { Play, Copy, Check, Settings, Code2, Zap } from 'lucide-react'

const exampleRequests = [
  {
    name: 'Create Queue',
    method: 'POST',
    endpoint: '/api/v1/queues',
    body: JSON.stringify({
      name: 'Customer Support',
      maxCapacity: 100,
      averageServiceTime: 5,
      priority: 'FIFO'
    }, null, 2),
  },
  {
    name: 'Generate Ticket',
    method: 'POST',
    endpoint: '/api/v1/tickets',
    body: JSON.stringify({
      queueId: 'queue_123',
      customer: {
        name: 'John Doe',
        email: 'john@example.com',
        phone: '+1234567890'
      },
      metadata: {
        issue: 'Technical Support',
        priority: 'high'
      }
    }, null, 2),
  },
  {
    name: 'List Queues',
    method: 'GET',
    endpoint: '/api/v1/queues',
    body: '',
  },
  {
    name: 'Get Queue Status',
    method: 'GET',
    endpoint: '/api/v1/queues/{id}/status',
    body: '',
  },
]

export default function PlaygroundPage() {
  const [apiKey, setApiKey] = useState('')
  const [selectedExample, setSelectedExample] = useState(exampleRequests[0])
  const [method, setMethod] = useState(selectedExample.method)
  const [endpoint, setEndpoint] = useState(selectedExample.endpoint)
  const [requestBody, setRequestBody] = useState(selectedExample.body)
  const [response, setResponse] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [copied, setCopied] = useState(false)

  const handleExampleSelect = (example: typeof exampleRequests[0]) => {
    setSelectedExample(example)
    setMethod(example.method)
    setEndpoint(example.endpoint)
    setRequestBody(example.body)
    setResponse('')
  }

  const handleSendRequest = async () => {
    if (!apiKey) {
      setResponse(JSON.stringify({ error: 'Please enter your API key' }, null, 2))
      return
    }

    setIsLoading(true)
    setResponse('')

    // Simulate API call
    setTimeout(() => {
      const mockResponse = {
        success: true,
        data: {
          id: 'queue_' + Math.random().toString(36).substr(2, 9),
          name: 'Customer Support',
          status: 'active',
          currentSize: 12,
          averageWaitTime: 5,
          createdAt: new Date().toISOString(),
        },
        message: 'Request successful'
      }
      setResponse(JSON.stringify(mockResponse, null, 2))
      setIsLoading(false)
    }, 1000)
  }

  const copyToClipboard = () => {
    const curlCommand = `curl -X ${method} https://api.queuemanagement.dev${endpoint} \\
  -H "Authorization: Bearer ${apiKey || 'YOUR_API_KEY'}" \\
  -H "Content-Type: application/json"${requestBody ? ` \\
  -d '${requestBody}'` : ''}`
    
    navigator.clipboard.writeText(curlCommand)
    setCopied(true)
    setTimeout(() => setCopied(false), 2000)
  }

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-950">
      <Navbar />
      
      <main className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 dark:text-white">
            API Playground
          </h1>
          <p className="mt-2 text-lg text-gray-600 dark:text-gray-300">
            Test API endpoints directly from your browser
          </p>
        </div>

        <div className="grid gap-8 lg:grid-cols-3">
          {/* Examples sidebar */}
          <div className="lg:col-span-1">
            <div className="rounded-lg bg-white p-6 shadow-lg dark:bg-gray-900">
              <h2 className="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
                Example Requests
              </h2>
              <div className="space-y-2">
                {exampleRequests.map((example) => (
                  <button
                    key={example.name}
                    onClick={() => handleExampleSelect(example)}
                    className={`w-full rounded-lg px-4 py-2 text-left text-sm transition-colors ${
                      selectedExample.name === example.name
                        ? 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400'
                        : 'text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-gray-800'
                    }`}
                  >
                    <div className="flex items-center justify-between">
                      <span>{example.name}</span>
                      <span
                        className={`text-xs font-semibold ${
                          example.method === 'GET'
                            ? 'text-green-600 dark:text-green-400'
                            : 'text-blue-600 dark:text-blue-400'
                        }`}
                      >
                        {example.method}
                      </span>
                    </div>
                  </button>
                ))}
              </div>
            </div>

            {/* API Key input */}
            <div className="mt-6 rounded-lg bg-white p-6 shadow-lg dark:bg-gray-900">
              <div className="flex items-center mb-2">
                <Settings className="mr-2 h-4 w-4 text-gray-600 dark:text-gray-400" />
                <h3 className="text-sm font-semibold text-gray-900 dark:text-white">
                  Configuration
                </h3>
              </div>
              <input
                type="password"
                placeholder="Enter your API key"
                value={apiKey}
                onChange={(e) => setApiKey(e.target.value)}
                className="w-full rounded-lg border bg-gray-50 px-4 py-2 text-sm focus:border-blue-500 focus:outline-none dark:bg-gray-800 dark:border-gray-700"
              />
              <p className="mt-2 text-xs text-gray-500 dark:text-gray-400">
                Your API key is never stored and only used for this session
              </p>
            </div>
          </div>

          {/* Request/Response area */}
          <div className="lg:col-span-2 space-y-6">
            {/* Request */}
            <div className="rounded-lg bg-white p-6 shadow-lg dark:bg-gray-900">
              <div className="mb-4 flex items-center justify-between">
                <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                  Request
                </h2>
                <button
                  onClick={copyToClipboard}
                  className="flex items-center space-x-2 rounded-lg bg-gray-100 px-3 py-1.5 text-sm text-gray-700 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-gray-700"
                >
                  {copied ? (
                    <>
                      <Check className="h-4 w-4" />
                      <span>Copied!</span>
                    </>
                  ) : (
                    <>
                      <Copy className="h-4 w-4" />
                      <span>Copy as cURL</span>
                    </>
                  )}
                </button>
              </div>

              <div className="space-y-4">
                <div className="flex space-x-4">
                  <select
                    value={method}
                    onChange={(e) => setMethod(e.target.value)}
                    className="rounded-lg border bg-gray-50 px-4 py-2 text-sm font-semibold focus:border-blue-500 focus:outline-none dark:bg-gray-800 dark:border-gray-700"
                  >
                    <option value="GET">GET</option>
                    <option value="POST">POST</option>
                    <option value="PUT">PUT</option>
                    <option value="DELETE">DELETE</option>
                  </select>
                  <input
                    type="text"
                    value={endpoint}
                    onChange={(e) => setEndpoint(e.target.value)}
                    className="flex-1 rounded-lg border bg-gray-50 px-4 py-2 text-sm focus:border-blue-500 focus:outline-none dark:bg-gray-800 dark:border-gray-700"
                  />
                </div>

                {method !== 'GET' && (
                  <div>
                    <label className="mb-2 block text-sm font-medium text-gray-700 dark:text-gray-300">
                      Request Body
                    </label>
                    <textarea
                      value={requestBody}
                      onChange={(e) => setRequestBody(e.target.value)}
                      rows={10}
                      className="w-full rounded-lg border bg-gray-50 px-4 py-2 font-mono text-sm focus:border-blue-500 focus:outline-none dark:bg-gray-800 dark:border-gray-700"
                    />
                  </div>
                )}

                <button
                  onClick={handleSendRequest}
                  disabled={isLoading}
                  className="flex w-full items-center justify-center rounded-lg bg-gradient-to-r from-blue-600 to-purple-600 px-4 py-3 text-sm font-semibold text-white hover:from-blue-700 hover:to-purple-700 disabled:opacity-50"
                >
                  {isLoading ? (
                    <>
                      <Zap className="mr-2 h-4 w-4 animate-pulse" />
                      Sending...
                    </>
                  ) : (
                    <>
                      <Play className="mr-2 h-4 w-4" />
                      Send Request
                    </>
                  )}
                </button>
              </div>
            </div>

            {/* Response */}
            {response && (
              <div className="rounded-lg bg-white p-6 shadow-lg dark:bg-gray-900">
                <h2 className="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
                  Response
                </h2>
                <div className="rounded-lg bg-gray-900 p-4">
                  <pre className="overflow-x-auto text-sm text-gray-300">
                    <code>{response}</code>
                  </pre>
                </div>
              </div>
            )}
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  )
}