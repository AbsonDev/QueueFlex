'use client'

import { motion } from 'framer-motion'
import { Monitor, Smartphone, Palette, Code2 } from 'lucide-react'
import Link from 'next/link'

const templates = [
  {
    name: 'Modern Dashboard',
    description: 'Full-featured admin dashboard with real-time updates',
    icon: Monitor,
    preview: '/templates/dashboard',
    github: 'https://github.com/queue-api/dashboard-template',
    technologies: ['React', 'TypeScript', 'Tailwind CSS'],
  },
  {
    name: 'Mobile App',
    description: 'React Native template for iOS and Android',
    icon: Smartphone,
    preview: '/templates/mobile',
    github: 'https://github.com/queue-api/mobile-template',
    technologies: ['React Native', 'Expo', 'TypeScript'],
  },
  {
    name: 'Customer Display',
    description: 'Digital signage template for waiting areas',
    icon: Palette,
    preview: '/templates/display',
    github: 'https://github.com/queue-api/display-template',
    technologies: ['Vue.js', 'WebSocket', 'CSS3'],
  },
  {
    name: 'Widget Library',
    description: 'Embeddable widgets for any website',
    icon: Code2,
    preview: '/templates/widgets',
    github: 'https://github.com/queue-api/widget-library',
    technologies: ['Web Components', 'JavaScript', 'CSS'],
  },
]

export function Templates() {
  return (
    <section id="templates" className="py-24 bg-white dark:bg-gray-950">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <motion.h2
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl dark:text-white"
          >
            Ready-to-Use Templates
          </motion.h2>
          <motion.p
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.1 }}
            className="mt-4 text-lg text-gray-600 dark:text-gray-300"
          >
            Customize and deploy beautiful interfaces in minutes
          </motion.p>
        </div>

        <div className="mt-16 grid gap-8 sm:grid-cols-2 lg:grid-cols-4">
          {templates.map((template, index) => (
            <motion.div
              key={template.name}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.1 }}
              className="group"
            >
              <div className="rounded-2xl border border-gray-200 bg-white p-6 transition-all hover:shadow-lg hover:-translate-y-1 dark:border-gray-800 dark:bg-gray-900">
                <div className="inline-flex h-12 w-12 items-center justify-center rounded-lg bg-gradient-to-br from-blue-100 to-purple-100 dark:from-blue-900/20 dark:to-purple-900/20">
                  <template.icon className="h-6 w-6 text-blue-600 dark:text-blue-400" />
                </div>
                
                <h3 className="mt-4 text-lg font-semibold text-gray-900 dark:text-white">
                  {template.name}
                </h3>
                
                <p className="mt-2 text-sm text-gray-600 dark:text-gray-300">
                  {template.description}
                </p>

                <div className="mt-4 flex flex-wrap gap-2">
                  {template.technologies.map((tech) => (
                    <span
                      key={tech}
                      className="inline-flex items-center rounded-full bg-gray-100 px-2.5 py-0.5 text-xs font-medium text-gray-700 dark:bg-gray-800 dark:text-gray-300"
                    >
                      {tech}
                    </span>
                  ))}
                </div>

                <div className="mt-6 flex space-x-3">
                  <Link
                    href={template.preview}
                    className="flex-1 rounded-lg bg-gray-900 px-3 py-2 text-center text-sm font-medium text-white hover:bg-gray-800 dark:bg-white dark:text-gray-900 dark:hover:bg-gray-100"
                  >
                    Preview
                  </Link>
                  <Link
                    href={template.github}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex-1 rounded-lg border border-gray-300 px-3 py-2 text-center text-sm font-medium text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-gray-800"
                  >
                    GitHub
                  </Link>
                </div>
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
          <Link
            href="/templates"
            className="inline-flex items-center text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300"
          >
            Browse all templates →
          </Link>
        </motion.div>
      </div>
    </section>
  )
}