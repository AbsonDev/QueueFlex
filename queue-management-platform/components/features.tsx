'use client'

import { motion } from 'framer-motion'
import { 
  Zap, Shield, Globe, BarChart3, Users, Code2, 
  Palette, Clock, Bell, Database, Cloud, Lock 
} from 'lucide-react'

const features = [
  {
    icon: Zap,
    title: 'Real-time Updates',
    description: 'WebSocket support for instant queue status updates and notifications',
    gradient: 'from-yellow-400 to-orange-500',
  },
  {
    icon: Shield,
    title: 'Enterprise Security',
    description: 'SOC 2 compliant with end-to-end encryption and role-based access control',
    gradient: 'from-green-400 to-emerald-500',
  },
  {
    icon: Globe,
    title: 'Global Infrastructure',
    description: 'Multi-region deployment with 99.99% uptime SLA guarantee',
    gradient: 'from-blue-400 to-cyan-500',
  },
  {
    icon: BarChart3,
    title: 'Advanced Analytics',
    description: 'Real-time metrics, custom reports, and predictive queue insights',
    gradient: 'from-purple-400 to-pink-500',
  },
  {
    icon: Users,
    title: 'Multi-tenant Ready',
    description: 'Built for SaaS with organization isolation and white-label support',
    gradient: 'from-indigo-400 to-purple-500',
  },
  {
    icon: Code2,
    title: 'Developer First',
    description: 'RESTful API, GraphQL, SDKs for all major languages',
    gradient: 'from-pink-400 to-rose-500',
  },
  {
    icon: Palette,
    title: 'Customizable UI',
    description: 'Pre-built React components and templates you can customize',
    gradient: 'from-teal-400 to-green-500',
  },
  {
    icon: Clock,
    title: 'Smart Scheduling',
    description: 'AI-powered queue optimization and automatic load balancing',
    gradient: 'from-orange-400 to-red-500',
  },
  {
    icon: Bell,
    title: 'Smart Notifications',
    description: 'Multi-channel alerts via email, SMS, push, and webhooks',
    gradient: 'from-cyan-400 to-blue-500',
  },
]

export function Features() {
  return (
    <section id="features" className="py-24 bg-white dark:bg-gray-950">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <motion.h2
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl dark:text-white"
          >
            Everything You Need to Build Queue Systems
          </motion.h2>
          <motion.p
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.1 }}
            className="mt-4 text-lg text-gray-600 dark:text-gray-300"
          >
            Powerful features designed for developers, loved by businesses
          </motion.p>
        </div>

        <div className="mt-20 grid gap-8 sm:grid-cols-2 lg:grid-cols-3">
          {features.map((feature, index) => (
            <motion.div
              key={feature.title}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.05 }}
              className="group relative"
            >
              <div className="rounded-2xl border border-gray-200 bg-white p-8 shadow-sm transition-all hover:shadow-xl hover:-translate-y-1 dark:border-gray-800 dark:bg-gray-900">
                <div className={`inline-flex h-12 w-12 items-center justify-center rounded-lg bg-gradient-to-br ${feature.gradient}`}>
                  <feature.icon className="h-6 w-6 text-white" />
                </div>
                
                <h3 className="mt-4 text-lg font-semibold text-gray-900 dark:text-white">
                  {feature.title}
                </h3>
                
                <p className="mt-2 text-gray-600 dark:text-gray-300">
                  {feature.description}
                </p>
                
                <div className="mt-4 flex items-center text-sm font-medium text-blue-600 dark:text-blue-400">
                  Learn more
                  <svg
                    className="ml-1 h-4 w-4 transition-transform group-hover:translate-x-1"
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
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  )
}