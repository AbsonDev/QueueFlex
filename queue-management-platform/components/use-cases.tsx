'use client'

import { motion } from 'framer-motion'
import { Building2, ShoppingCart, Stethoscope, GraduationCap, Plane, Utensils } from 'lucide-react'

const useCases = [
  {
    icon: Building2,
    title: 'Banks & Financial',
    description: 'Manage customer queues in branches, reduce wait times, and improve service efficiency.',
    image: '/images/bank.jpg',
  },
  {
    icon: Stethoscope,
    title: 'Healthcare',
    description: 'Patient queue management for clinics, hospitals, and medical centers.',
    image: '/images/healthcare.jpg',
  },
  {
    icon: ShoppingCart,
    title: 'Retail',
    description: 'Handle customer service desks, returns, and special services efficiently.',
    image: '/images/retail.jpg',
  },
  {
    icon: GraduationCap,
    title: 'Education',
    description: 'Student services, enrollment, and administrative queue management.',
    image: '/images/education.jpg',
  },
  {
    icon: Plane,
    title: 'Travel & Tourism',
    description: 'Check-in counters, information desks, and customer service management.',
    image: '/images/travel.jpg',
  },
  {
    icon: Utensils,
    title: 'Restaurants',
    description: 'Waitlist management, table reservations, and takeout order queues.',
    image: '/images/restaurant.jpg',
  },
]

export function UseCases() {
  return (
    <section id="use-cases" className="py-24 bg-gray-50 dark:bg-gray-900/50">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <motion.h2
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl dark:text-white"
          >
            Built for Every Industry
          </motion.h2>
          <motion.p
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.1 }}
            className="mt-4 text-lg text-gray-600 dark:text-gray-300"
          >
            Trusted by thousands of businesses worldwide
          </motion.p>
        </div>

        <div className="mt-16 grid gap-8 sm:grid-cols-2 lg:grid-cols-3">
          {useCases.map((useCase, index) => (
            <motion.div
              key={useCase.title}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.1 }}
              className="group cursor-pointer"
            >
              <div className="overflow-hidden rounded-2xl bg-white shadow-lg transition-all hover:shadow-xl hover:-translate-y-1 dark:bg-gray-800">
                <div className="aspect-w-16 aspect-h-9 bg-gradient-to-br from-blue-100 to-purple-100 dark:from-blue-900/20 dark:to-purple-900/20">
                  <div className="flex items-center justify-center h-48">
                    <useCase.icon className="h-16 w-16 text-blue-600 dark:text-blue-400" />
                  </div>
                </div>
                <div className="p-6">
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white">
                    {useCase.title}
                  </h3>
                  <p className="mt-2 text-sm text-gray-600 dark:text-gray-300">
                    {useCase.description}
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
              </div>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  )
}