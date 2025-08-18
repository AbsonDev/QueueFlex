'use client'

import { motion } from 'framer-motion'
import { Users, Zap, Globe, TrendingUp } from 'lucide-react'

const stats = [
  {
    icon: Users,
    value: '10,000+',
    label: 'Active Developers',
    description: 'Building with our API',
  },
  {
    icon: Zap,
    value: '50M+',
    label: 'API Calls',
    description: 'Processed monthly',
  },
  {
    icon: Globe,
    value: '99.99%',
    label: 'Uptime SLA',
    description: 'Guaranteed availability',
  },
  {
    icon: TrendingUp,
    value: '<100ms',
    label: 'Response Time',
    description: 'Average API latency',
  },
]

export function Stats() {
  return (
    <section className="py-24 bg-gradient-to-r from-blue-600 to-purple-600">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <motion.h2
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-3xl font-bold tracking-tight text-white sm:text-4xl"
          >
            Trusted by Developers Worldwide
          </motion.h2>
          <motion.p
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.1 }}
            className="mt-4 text-lg text-blue-100"
          >
            Join thousands of companies using our platform
          </motion.p>
        </div>

        <div className="mt-16 grid gap-8 sm:grid-cols-2 lg:grid-cols-4">
          {stats.map((stat, index) => (
            <motion.div
              key={stat.label}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.1 }}
              className="text-center"
            >
              <div className="inline-flex h-12 w-12 items-center justify-center rounded-lg bg-white/10 backdrop-blur-sm">
                <stat.icon className="h-6 w-6 text-white" />
              </div>
              <div className="mt-4">
                <div className="text-4xl font-bold text-white">{stat.value}</div>
                <div className="mt-2 text-lg font-medium text-blue-100">{stat.label}</div>
                <div className="mt-1 text-sm text-blue-200">{stat.description}</div>
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  )
}