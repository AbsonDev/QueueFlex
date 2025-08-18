'use client'

import { motion } from 'framer-motion'
import Link from 'next/link'
import { ArrowRight, Play, Star, Users, Zap, Shield, Code2 } from 'lucide-react'

export function Hero() {
  return (
    <section className="relative overflow-hidden pt-20 pb-32">
      {/* Background gradient */}
      <div className="absolute inset-0 -z-10">
        <div className="absolute inset-0 bg-gradient-to-br from-blue-50 via-white to-purple-50 dark:from-gray-950 dark:via-gray-900 dark:to-gray-950" />
        <div className="absolute inset-y-0 right-0 w-1/2 bg-gradient-to-l from-purple-100/20 to-transparent dark:from-purple-900/10" />
      </div>

      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          {/* Badge */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
            className="inline-flex items-center rounded-full bg-blue-100 px-4 py-1.5 text-sm font-medium text-blue-700 dark:bg-blue-900/30 dark:text-blue-400"
          >
            <Zap className="mr-2 h-4 w-4" />
            v2.0 Released - Real-time WebSocket Support
          </motion.div>

          {/* Heading */}
          <motion.h1
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: 0.1 }}
            className="mt-8 text-5xl font-bold tracking-tight text-gray-900 sm:text-6xl lg:text-7xl dark:text-white"
          >
            Queue Management API
            <span className="block bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              for Modern Applications
            </span>
          </motion.h1>

          {/* Description */}
          <motion.p
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: 0.2 }}
            className="mx-auto mt-6 max-w-2xl text-lg leading-8 text-gray-600 dark:text-gray-300"
          >
            Enterprise-grade queue management system with customizable UI templates. 
            Build scalable customer service solutions in minutes, not months.
          </motion.p>

          {/* CTA Buttons */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: 0.3 }}
            className="mt-10 flex items-center justify-center gap-x-6"
          >
            <Link
              href="/signup"
              className="group inline-flex items-center rounded-lg bg-gradient-to-r from-blue-600 to-purple-600 px-6 py-3 text-sm font-semibold text-white shadow-lg hover:from-blue-700 hover:to-purple-700 transition-all duration-200"
            >
              Start Free Trial
              <ArrowRight className="ml-2 h-4 w-4 group-hover:translate-x-1 transition-transform" />
            </Link>
            <Link
              href="/docs"
              className="group inline-flex items-center rounded-lg bg-white px-6 py-3 text-sm font-semibold text-gray-900 shadow-md ring-1 ring-gray-200 hover:bg-gray-50 dark:bg-gray-800 dark:text-white dark:ring-gray-700 dark:hover:bg-gray-700"
            >
              <Play className="mr-2 h-4 w-4" />
              View Demo
            </Link>
          </motion.div>

          {/* Trust badges */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: 0.4 }}
            className="mt-12 flex items-center justify-center space-x-8 text-sm text-gray-600 dark:text-gray-400"
          >
            <div className="flex items-center">
              <Users className="mr-2 h-5 w-5 text-blue-600" />
              <span>10,000+ Developers</span>
            </div>
            <div className="flex items-center">
              <Star className="mr-2 h-5 w-5 text-yellow-500" />
              <span>4.9/5 Rating</span>
            </div>
            <div className="flex items-center">
              <Shield className="mr-2 h-5 w-5 text-green-600" />
              <span>SOC 2 Compliant</span>
            </div>
          </motion.div>

          {/* Code preview */}
          <motion.div
            initial={{ opacity: 0, y: 40 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.7, delay: 0.5 }}
            className="mt-16 relative"
          >
            <div className="relative mx-auto max-w-4xl">
              <div className="absolute inset-0 bg-gradient-to-r from-blue-400 to-purple-400 blur-3xl opacity-20" />
              <div className="relative rounded-xl bg-gray-900 p-4 shadow-2xl ring-1 ring-white/10">
                <div className="flex items-center space-x-2 mb-4">
                  <div className="h-3 w-3 rounded-full bg-red-500" />
                  <div className="h-3 w-3 rounded-full bg-yellow-500" />
                  <div className="h-3 w-3 rounded-full bg-green-500" />
                  <span className="ml-4 text-xs text-gray-400">quick-start.js</span>
                </div>
                <pre className="text-sm text-gray-300">
                  <code>{`import { QueueAPI } from '@queue-api/sdk';

const api = new QueueAPI({ apiKey: 'your-api-key' });

// Create a new queue
const queue = await api.queues.create({
  name: 'Customer Service',
  maxCapacity: 100,
  averageServiceTime: 5
});

// Generate a ticket
const ticket = await api.tickets.generate({
  queueId: queue.id,
  customer: { name: 'John Doe', email: 'john@example.com' }
});

console.log(\`Ticket \${ticket.number} created. Position: \${ticket.position}\`);`}</code>
                </pre>
              </div>
            </div>
          </motion.div>
        </div>
      </div>
    </section>
  )
}