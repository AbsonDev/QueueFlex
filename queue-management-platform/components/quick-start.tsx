'use client'

import { motion } from 'framer-motion'
import { Rocket, Key, Code2, CheckCircle } from 'lucide-react'

const steps = [
  {
    icon: Key,
    title: 'Get Your API Key',
    description: 'Sign up for free and get instant access to your API credentials',
    code: `curl -X POST https://api.queuemanagement.dev/v1/auth/register \\
  -H "Content-Type: application/json" \\
  -d '{"email": "you@example.com", "password": "secure_password"}'`,
  },
  {
    icon: Code2,
    title: 'Install SDK',
    description: 'Choose your preferred language and install our official SDK',
    code: `npm install @queue-api/sdk
# or
pip install queue-management-sdk
# or
composer require queue-api/sdk`,
  },
  {
    icon: Rocket,
    title: 'Start Building',
    description: 'Create your first queue and start managing tickets in minutes',
    code: `const queue = await api.queues.create({
  name: 'Support Queue',
  maxCapacity: 50
});`,
  },
]

export function QuickStart() {
  return (
    <section id="quick-start" className="py-24 bg-gray-50 dark:bg-gray-900/50">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <motion.h2
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl dark:text-white"
          >
            Get Started in 3 Simple Steps
          </motion.h2>
          <motion.p
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.1 }}
            className="mt-4 text-lg text-gray-600 dark:text-gray-300"
          >
            From zero to production-ready queue management in under 5 minutes
          </motion.p>
        </div>

        <div className="mt-16 grid gap-8 lg:grid-cols-3">
          {steps.map((step, index) => (
            <motion.div
              key={step.title}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.1 }}
              className="relative"
            >
              <div className="rounded-2xl bg-white p-8 shadow-lg ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-700">
                <div className="flex items-center justify-between mb-4">
                  <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-gradient-to-br from-blue-600 to-purple-600">
                    <step.icon className="h-6 w-6 text-white" />
                  </div>
                  <span className="text-4xl font-bold text-gray-200 dark:text-gray-700">
                    {index + 1}
                  </span>
                </div>
                
                <h3 className="text-xl font-semibold text-gray-900 dark:text-white mb-2">
                  {step.title}
                </h3>
                <p className="text-gray-600 dark:text-gray-300 mb-4">
                  {step.description}
                </p>
                
                <div className="rounded-lg bg-gray-900 p-4">
                  <pre className="text-xs text-gray-300 overflow-x-auto">
                    <code>{step.code}</code>
                  </pre>
                </div>
                
                {index < steps.length - 1 && (
                  <div className="absolute top-1/2 -right-4 hidden lg:block">
                    <svg
                      className="h-8 w-8 text-gray-300 dark:text-gray-600"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M9 5l7 7-7 7"
                      />
                    </svg>
                  </div>
                )}
              </div>
            </motion.div>
          ))}
        </div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          className="mt-12 text-center"
        >
          <div className="inline-flex items-center rounded-full bg-green-100 px-4 py-2 text-sm font-medium text-green-700 dark:bg-green-900/30 dark:text-green-400">
            <CheckCircle className="mr-2 h-4 w-4" />
            Free tier includes 10,000 API calls per month
          </div>
        </motion.div>
      </div>
    </section>
  )
}