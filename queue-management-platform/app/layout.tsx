import type { Metadata } from 'next'
import { Inter } from 'next/font/google'
import './globals.css'
import { ThemeProvider } from '@/components/theme-provider'

const inter = Inter({ subsets: ['latin'] })

export const metadata: Metadata = {
  title: 'Queue Management API - Developer Platform',
  description: 'Enterprise-grade Queue Management API with customizable UI templates. Build scalable queue systems in minutes.',
  keywords: 'queue management, api, saas, developer platform, queue system, ticket system',
  authors: [{ name: 'Queue Management Team' }],
  openGraph: {
    title: 'Queue Management API - Developer Platform',
    description: 'Enterprise-grade Queue Management API with customizable UI templates',
    type: 'website',
    url: 'https://queueapi.dev',
    images: [
      {
        url: '/og-image.png',
        width: 1200,
        height: 630,
        alt: 'Queue Management API',
      },
    ],
  },
  twitter: {
    card: 'summary_large_image',
    title: 'Queue Management API',
    description: 'Enterprise-grade Queue Management API with customizable UI templates',
    images: ['/og-image.png'],
  },
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className={inter.className}>
        <ThemeProvider
          attribute="class"
          defaultTheme="system"
          enableSystem
          disableTransitionOnChange
        >
          {children}
        </ThemeProvider>
      </body>
    </html>
  )
}