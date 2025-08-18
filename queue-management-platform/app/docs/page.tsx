'use client'

import { useState } from 'react'
import Link from 'next/link'
import { Navbar } from '@/components/navbar'
import { Footer } from '@/components/footer'
import { 
  BookOpen, Code2, Zap, Shield, Database, Globe, 
  ChevronRight, Search, Copy, Check, ExternalLink 
} from 'lucide-react'

const sidebarItems = [
  {
    title: 'Getting Started',
    items: [
      { name: 'Introduction', href: '#introduction' },
      { name: 'Quick Start', href: '#quick-start' },
      { name: 'Authentication', href: '#authentication' },
      { name: 'Rate Limits', href: '#rate-limits' },
    ],
  },
  {
    title: 'API Reference',
    items: [
      { name: 'Queues', href: '#queues' },
      { name: 'Tickets', href: '#tickets' },
      { name: 'Customers', href: '#customers' },
      { name: 'Analytics', href: '#analytics' },
      { name: 'Webhooks', href: '#webhooks' },
    ],
  },
  {
    title: 'SDKs & Libraries',
    items: [
      { name: 'JavaScript/Node.js', href: '#sdk-javascript' },
      { name: 'Python', href: '#sdk-python' },
      { name: 'PHP', href: '#sdk-php' },
      { name: 'Java', href: '#sdk-java' },
      { name: 'C#/.NET', href: '#sdk-csharp' },
      { name: 'Go', href: '#sdk-go' },
    ],
  },
  {
    title: 'Guides',
    items: [
      { name: 'Real-time Updates', href: '#realtime' },
      { name: 'Multi-tenancy', href: '#multi-tenancy' },
      { name: 'Custom Workflows', href: '#workflows' },
      { name: 'Scaling Best Practices', href: '#scaling' },
    ],
  },
  {
    title: 'Resources',
    items: [
      { name: 'Changelog', href: '#changelog' },
      { name: 'Status Page', href: '#status' },
      { name: 'Support', href: '#support' },
    ],
  },
]

const apiEndpoints = [
  {
    method: 'GET',
    path: '/api/v1/queues',
    description: 'List all queues',
  },
  {
    method: 'POST',
    path: '/api/v1/queues',
    description: 'Create a new queue',
  },
  {
    method: 'GET',
    path: '/api/v1/queues/{id}',
    description: 'Get queue details',
  },
  {
    method: 'PUT',
    path: '/api/v1/queues/{id}',
    description: 'Update queue',
  },
  {
    method: 'DELETE',
    path: '/api/v1/queues/{id}',
    description: 'Delete queue',
  },
  {
    method: 'POST',
    path: '/api/v1/tickets',
    description: 'Generate a ticket',
  },
  {
    method: 'GET',
    path: '/api/v1/tickets/{id}',
    description: 'Get ticket details',
  },
  {
    method: 'POST',
    path: '/api/v1/tickets/{id}/call',
    description: 'Call next ticket',
  },
]

export default function DocsPage() {
  const [searchQuery, setSearchQuery] = useState('')
  const [copiedCode, setCopiedCode] = useState<string | null>(null)

  const copyCode = (code: string, id: string) => {
    navigator.clipboard.writeText(code)
    setCopiedCode(id)
    setTimeout(() => setCopiedCode(null), 2000)
  }

  return (
    <div className="min-h-screen bg-white dark:bg-gray-950">
      <Navbar />
      
      <div className="flex">
        {/* Sidebar */}
        <aside className="fixed left-0 top-16 h-[calc(100vh-4rem)] w-64 overflow-y-auto border-r bg-gray-50 p-6 dark:bg-gray-900 dark:border-gray-800">
          <div className="mb-6">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
              <input
                type="text"
                placeholder="Search docs..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full rounded-lg border bg-white py-2 pl-10 pr-4 text-sm focus:border-blue-500 focus:outline-none dark:bg-gray-800 dark:border-gray-700"
              />
            </div>
          </div>

          <nav className="space-y-6">
            {sidebarItems.map((section) => (
              <div key={section.title}>
                <h3 className="mb-2 text-sm font-semibold text-gray-900 dark:text-white">
                  {section.title}
                </h3>
                <ul className="space-y-1">
                  {section.items.map((item) => (
                    <li key={item.name}>
                      <Link
                        href={item.href}
                        className="block rounded-lg px-3 py-2 text-sm text-gray-600 hover:bg-gray-100 hover:text-gray-900 dark:text-gray-400 dark:hover:bg-gray-800 dark:hover:text-white"
                      >
                        {item.name}
                      </Link>
                    </li>
                  ))}
                </ul>
              </div>
            ))}
          </nav>
        </aside>

        {/* Main content */}
        <main className="ml-64 flex-1 p-8">
          <div className="mx-auto max-w-4xl">
            {/* Introduction */}
            <section id="introduction" className="mb-12">
              <h1 className="text-4xl font-bold text-gray-900 dark:text-white">
                API Documentation
              </h1>
              <p className="mt-4 text-lg text-gray-600 dark:text-gray-300">
                Complete reference for the Queue Management API
              </p>
            </section>

            {/* Quick Start */}
            <section id="quick-start" className="mb-12">
              <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                Quick Start
              </h2>
              
              <div className="space-y-6">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                    1. Get your API key
                  </h3>
                  <p className="text-gray-600 dark:text-gray-300 mb-4">
                    Sign up for an account and get your API key from the dashboard.
                  </p>
                </div>

                <div>
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                    2. Install the SDK
                  </h3>
                  <div className="relative rounded-lg bg-gray-900 p-4">
                    <button
                      onClick={() => copyCode('npm install @queue-api/sdk', 'install')}
                      className="absolute right-2 top-2 rounded p-2 text-gray-400 hover:bg-gray-800 hover:text-white"
                    >
                      {copiedCode === 'install' ? (
                        <Check className="h-4 w-4" />
                      ) : (
                        <Copy className="h-4 w-4" />
                      )}
                    </button>
                    <pre className="text-sm text-gray-300">
                      <code>npm install @queue-api/sdk</code>
                    </pre>
                  </div>
                </div>

                <div>
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                    3. Initialize the client
                  </h3>
                  <div className="relative rounded-lg bg-gray-900 p-4">
                    <button
                      onClick={() => copyCode(`import { QueueAPI } from '@queue-api/sdk';

const api = new QueueAPI({
  apiKey: 'your-api-key'
});`, 'init')}
                      className="absolute right-2 top-2 rounded p-2 text-gray-400 hover:bg-gray-800 hover:text-white"
                    >
                      {copiedCode === 'init' ? (
                        <Check className="h-4 w-4" />
                      ) : (
                        <Copy className="h-4 w-4" />
                      )}
                    </button>
                    <pre className="text-sm text-gray-300">
                      <code>{`import { QueueAPI } from '@queue-api/sdk';

const api = new QueueAPI({
  apiKey: 'your-api-key'
});`}</code>
                    </pre>
                  </div>
                </div>
              </div>
            </section>

            {/* API Endpoints */}
            <section id="queues" className="mb-12">
              <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                API Endpoints
              </h2>
              
              <div className="space-y-4">
                {apiEndpoints.map((endpoint, index) => (
                  <div
                    key={index}
                    className="rounded-lg border bg-white p-4 dark:bg-gray-900 dark:border-gray-800"
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex items-center space-x-3">
                        <span
                          className={`rounded px-2 py-1 text-xs font-semibold ${
                            endpoint.method === 'GET'
                              ? 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400'
                              : endpoint.method === 'POST'
                              ? 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400'
                              : endpoint.method === 'PUT'
                              ? 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400'
                              : 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400'
                          }`}
                        >
                          {endpoint.method}
                        </span>
                        <code className="text-sm font-mono text-gray-700 dark:text-gray-300">
                          {endpoint.path}
                        </code>
                      </div>
                      <ChevronRight className="h-4 w-4 text-gray-400" />
                    </div>
                    <p className="mt-2 text-sm text-gray-600 dark:text-gray-400">
                      {endpoint.description}
                    </p>
                  </div>
                ))}
              </div>
            </section>

            {/* Authentication */}
            <section id="authentication" className="mb-12">
              <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                Authentication
              </h2>
              <p className="text-gray-600 dark:text-gray-300 mb-4">
                The Queue Management API uses API keys to authenticate requests. 
                You can view and manage your API keys in your dashboard.
              </p>
              
              <div className="rounded-lg bg-yellow-50 border border-yellow-200 p-4 dark:bg-yellow-900/20 dark:border-yellow-800">
                <div className="flex">
                  <Shield className="h-5 w-5 text-yellow-600 dark:text-yellow-400" />
                  <div className="ml-3">
                    <h3 className="text-sm font-medium text-yellow-800 dark:text-yellow-200">
                      Security Notice
                    </h3>
                    <p className="mt-1 text-sm text-yellow-700 dark:text-yellow-300">
                      Keep your API keys secure and never share them in publicly accessible areas.
                    </p>
                  </div>
                </div>
              </div>

              <div className="mt-6">
                <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                  Using API Keys
                </h3>
                <p className="text-gray-600 dark:text-gray-300 mb-4">
                  Include your API key in the Authorization header:
                </p>
                <div className="relative rounded-lg bg-gray-900 p-4">
                  <pre className="text-sm text-gray-300">
                    <code>Authorization: Bearer YOUR_API_KEY</code>
                  </pre>
                </div>
              </div>
            </section>

            {/* Rate Limits */}
            <section id="rate-limits" className="mb-12">
              <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                Rate Limits
              </h2>
              
              <div className="overflow-x-auto">
                <table className="w-full border-collapse">
                  <thead>
                    <tr className="border-b dark:border-gray-800">
                      <th className="text-left py-3 px-4 text-sm font-semibold text-gray-900 dark:text-white">
                        Plan
                      </th>
                      <th className="text-left py-3 px-4 text-sm font-semibold text-gray-900 dark:text-white">
                        Requests/Second
                      </th>
                      <th className="text-left py-3 px-4 text-sm font-semibold text-gray-900 dark:text-white">
                        Requests/Month
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr className="border-b dark:border-gray-800">
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">Hobby</td>
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">10</td>
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">10,000</td>
                    </tr>
                    <tr className="border-b dark:border-gray-800">
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">Pro</td>
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">100</td>
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">1,000,000</td>
                    </tr>
                    <tr>
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">Enterprise</td>
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">Custom</td>
                      <td className="py-3 px-4 text-sm text-gray-600 dark:text-gray-300">Unlimited</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </section>
          </div>
        </main>
      </div>
      
      <Footer />
    </div>
  )
}