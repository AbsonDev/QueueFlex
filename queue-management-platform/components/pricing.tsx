'use client'

import { useState } from 'react'
import { motion } from 'framer-motion'
import { Check, X, Zap, Shield, Globe } from 'lucide-react'
import Link from 'next/link'

const plans = [
  {
    name: 'Hobby',
    price: 0,
    description: 'Perfect for side projects and testing',
    icon: Zap,
    color: 'from-green-400 to-emerald-500',
    features: [
      '10,000 API calls/month',
      '2 queues',
      '100 concurrent tickets',
      'Community support',
      'Basic analytics',
      '7-day data retention',
    ],
    notIncluded: [
      'Custom domain',
      'SLA guarantee',
      'Priority support',
      'Advanced analytics',
    ],
  },
  {
    name: 'Pro',
    price: 99,
    description: 'For growing businesses and teams',
    icon: Shield,
    color: 'from-blue-400 to-purple-500',
    popular: true,
    features: [
      '1,000,000 API calls/month',
      'Unlimited queues',
      '10,000 concurrent tickets',
      'Priority email support',
      'Advanced analytics',
      '90-day data retention',
      'Custom domain',
      '99.9% SLA',
      'Webhooks',
      'Team collaboration',
    ],
    notIncluded: [
      'Dedicated support',
      'Custom integrations',
    ],
  },
  {
    name: 'Enterprise',
    price: 'Custom',
    description: 'Tailored solutions for large organizations',
    icon: Globe,
    color: 'from-purple-400 to-pink-500',
    features: [
      'Unlimited API calls',
      'Unlimited everything',
      'Dedicated support team',
      'Custom integrations',
      'White-label solution',
      'Unlimited data retention',
      'Custom domain',
      '99.99% SLA',
      'Advanced security',
      'On-premise deployment',
      'Custom contract',
      'Training & onboarding',
    ],
    notIncluded: [],
  },
]

export function Pricing() {
  const [billingPeriod, setBillingPeriod] = useState<'monthly' | 'yearly'>('monthly')

  return (
    <section id="pricing" className="py-24 bg-white dark:bg-gray-950">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <motion.h2
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl dark:text-white"
          >
            Simple, Transparent Pricing
          </motion.h2>
          <motion.p
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.1 }}
            className="mt-4 text-lg text-gray-600 dark:text-gray-300"
          >
            Start free, scale as you grow. No hidden fees.
          </motion.p>

          {/* Billing toggle */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            transition={{ delay: 0.2 }}
            className="mt-8 flex items-center justify-center space-x-4"
          >
            <span className={billingPeriod === 'monthly' ? 'text-gray-900 dark:text-white' : 'text-gray-500'}>
              Monthly
            </span>
            <button
              onClick={() => setBillingPeriod(billingPeriod === 'monthly' ? 'yearly' : 'monthly')}
              className="relative inline-flex h-6 w-11 items-center rounded-full bg-gray-200 transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 dark:bg-gray-700"
            >
              <span
                className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
                  billingPeriod === 'yearly' ? 'translate-x-6' : 'translate-x-1'
                }`}
              />
            </button>
            <span className={billingPeriod === 'yearly' ? 'text-gray-900 dark:text-white' : 'text-gray-500'}>
              Yearly
              <span className="ml-2 inline-flex items-center rounded-full bg-green-100 px-2 py-0.5 text-xs font-medium text-green-700 dark:bg-green-900/30 dark:text-green-400">
                Save 20%
              </span>
            </span>
          </motion.div>
        </div>

        <div className="mt-16 grid gap-8 lg:grid-cols-3">
          {plans.map((plan, index) => (
            <motion.div
              key={plan.name}
              initial={{ opacity: 0, y: 20 }}
              whileInView={{ opacity: 1, y: 0 }}
              viewport={{ once: true }}
              transition={{ delay: index * 0.1 }}
              className={`relative rounded-2xl ${
                plan.popular
                  ? 'ring-2 ring-blue-600 dark:ring-blue-400'
                  : 'ring-1 ring-gray-200 dark:ring-gray-800'
              }`}
            >
              {plan.popular && (
                <div className="absolute -top-4 left-1/2 -translate-x-1/2">
                  <span className="inline-flex items-center rounded-full bg-gradient-to-r from-blue-600 to-purple-600 px-4 py-1 text-sm font-medium text-white">
                    Most Popular
                  </span>
                </div>
              )}

              <div className="p-8 bg-white dark:bg-gray-900 rounded-2xl">
                <div className="flex items-center justify-between">
                  <h3 className="text-2xl font-bold text-gray-900 dark:text-white">
                    {plan.name}
                  </h3>
                  <div className={`inline-flex h-10 w-10 items-center justify-center rounded-lg bg-gradient-to-br ${plan.color}`}>
                    <plan.icon className="h-5 w-5 text-white" />
                  </div>
                </div>

                <p className="mt-4 text-gray-600 dark:text-gray-300">
                  {plan.description}
                </p>

                <div className="mt-6">
                  {typeof plan.price === 'number' ? (
                    <div className="flex items-baseline">
                      <span className="text-4xl font-bold text-gray-900 dark:text-white">
                        ${billingPeriod === 'yearly' ? Math.floor(plan.price * 0.8) : plan.price}
                      </span>
                      <span className="ml-2 text-gray-600 dark:text-gray-400">
                        /month
                      </span>
                    </div>
                  ) : (
                    <div className="text-4xl font-bold text-gray-900 dark:text-white">
                      {plan.price}
                    </div>
                  )}
                </div>

                <Link
                  href={plan.name === 'Enterprise' ? '/contact' : '/signup'}
                  className={`mt-8 block w-full rounded-lg px-4 py-3 text-center text-sm font-semibold transition-all ${
                    plan.popular
                      ? 'bg-gradient-to-r from-blue-600 to-purple-600 text-white hover:from-blue-700 hover:to-purple-700'
                      : 'bg-gray-900 text-white hover:bg-gray-800 dark:bg-white dark:text-gray-900 dark:hover:bg-gray-100'
                  }`}
                >
                  {plan.name === 'Enterprise' ? 'Contact Sales' : 'Get Started'}
                </Link>

                <ul className="mt-8 space-y-4">
                  {plan.features.map((feature) => (
                    <li key={feature} className="flex items-start">
                      <Check className="mr-3 h-5 w-5 flex-shrink-0 text-green-500" />
                      <span className="text-sm text-gray-700 dark:text-gray-300">
                        {feature}
                      </span>
                    </li>
                  ))}
                  {plan.notIncluded.map((feature) => (
                    <li key={feature} className="flex items-start opacity-50">
                      <X className="mr-3 h-5 w-5 flex-shrink-0 text-gray-400" />
                      <span className="text-sm text-gray-500 dark:text-gray-500 line-through">
                        {feature}
                      </span>
                    </li>
                  ))}
                </ul>
              </div>
            </motion.div>
          ))}
        </div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          className="mt-12 text-center text-sm text-gray-600 dark:text-gray-400"
        >
          All plans include SSL encryption, API documentation, and basic support.
          <br />
          Need a custom plan?{' '}
          <Link href="/contact" className="text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300">
            Contact our sales team
          </Link>
        </motion.div>
      </div>
    </section>
  )
}