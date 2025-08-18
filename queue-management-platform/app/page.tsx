'use client'

import { useState } from 'react'
import { motion } from 'framer-motion'
import { Navbar } from '@/components/navbar'
import { Hero } from '@/components/hero'
import { QuickStart } from '@/components/quick-start'
import { Features } from '@/components/features'
import { CodeExamples } from '@/components/code-examples'
import { Pricing } from '@/components/pricing'
import { Stats } from '@/components/stats'
import { Footer } from '@/components/footer'
import { UseCases } from '@/components/use-cases'
import { Templates } from '@/components/templates'

export default function Home() {
  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-50 to-white dark:from-gray-950 dark:to-gray-900">
      <Navbar />
      
      <main>
        <Hero />
        <QuickStart />
        <Features />
        <CodeExamples />
        <UseCases />
        <Templates />
        <Stats />
        <Pricing />
      </main>
      
      <Footer />
    </div>
  )
}