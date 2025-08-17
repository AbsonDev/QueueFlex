# Queue Management Admin Dashboard

A modern, responsive admin dashboard for Queue Management Systems built with React, TypeScript, and Tailwind CSS.

## 🚀 Features

- **Real-time Dashboard** - Live updates via SignalR
- **Queue Management** - Create, monitor, and manage queues
- **Ticket System** - Generate and track tickets
- **User Management** - Manage users, roles, and permissions
- **Analytics** - Performance metrics and reporting
- **Multi-tenant Support** - Support for multiple organizations
- **Responsive Design** - Mobile-first approach
- **Dark/Light Theme** - Customizable appearance
- **Real-time Notifications** - Toast notifications and alerts

## 🛠️ Tech Stack

- **Frontend Framework**: React 18 with TypeScript
- **Build Tool**: Vite
- **Styling**: Tailwind CSS
- **State Management**: Zustand
- **Data Fetching**: React Query (TanStack Query)
- **Real-time**: SignalR
- **Routing**: React Router v6
- **Forms**: React Hook Form + Zod
- **Icons**: Lucide React
- **Charts**: Recharts
- **Notifications**: Sonner
- **Animations**: Framer Motion

## 📦 Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd queuemanagement-admin
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Set up environment variables**
   ```bash
   cp .env.example .env.local
   ```
   
   Edit `.env.local` with your configuration:
   ```env
   VITE_API_URL=http://localhost:5000/api/v1
   VITE_TENANT_ID=your-tenant-id
   ```

4. **Start development server**
   ```bash
   npm run dev
   ```

5. **Open your browser**
   Navigate to [http://localhost:3000](http://localhost:3000)

## 🔧 Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint
- `npm run type-check` - Run TypeScript type checking

## 🏗️ Project Structure

```
src/
├── components/          # Reusable UI components
│   ├── ui/             # Base UI components (Button, Input, Card, etc.)
│   ├── layout/         # Layout components (Header, Sidebar, etc.)
│   ├── forms/          # Form components
│   └── charts/         # Chart components
├── pages/              # Page components
├── hooks/              # Custom React hooks
├── lib/                # Utility functions and configurations
├── services/           # API service functions
├── stores/             # Zustand state stores
├── types/              # TypeScript type definitions
└── styles/             # Global styles and CSS
```

## 🎨 Component Library

The project includes a comprehensive component library built with Tailwind CSS:

- **Button** - Multiple variants (default, destructive, outline, etc.)
- **Input** - Form inputs with validation states
- **Card** - Content containers with header, content, and footer
- **Badge** - Status indicators and labels
- **MetricCard** - Dashboard metric displays
- **Modal** - Overlay dialogs and forms
- **Table** - Data tables with sorting and pagination
- **Charts** - Data visualization components

## 🔐 Authentication

The dashboard includes a complete authentication system:

- JWT-based authentication
- Automatic token refresh
- Protected routes
- Role-based access control
- Multi-tenant support

## 📊 Real-time Features

Real-time updates are powered by SignalR:

- Live queue status updates
- Real-time ticket notifications
- Dashboard metrics updates
- User status changes
- Session updates

## 🎯 Key Pages

### Dashboard
- Overview metrics and KPIs
- Real-time queue status
- Performance indicators
- Top performers
- Today's statistics

### Queue Management
- Queue overview and status
- Create and edit queues
- Real-time monitoring
- Capacity management

### Ticket System
- Ticket generation
- Status tracking
- Assignment management
- History and analytics

### User Management
- User CRUD operations
- Role and permission management
- Performance tracking
- Status monitoring

## 🚀 Deployment

### Build for Production
```bash
npm run build
```

### Environment Configuration
Ensure all environment variables are properly set for production:

```env
VITE_API_URL=https://your-api-domain.com/api/v1
VITE_TENANT_ID=production-tenant
VITE_SIGNALR_URL=https://your-api-domain.com/hubs
```

### Deployment Platforms
The dashboard can be deployed to any static hosting platform:

- **Vercel** - Recommended for React apps
- **Netlify** - Great for static sites
- **AWS S3 + CloudFront** - Enterprise solution
- **GitHub Pages** - Free hosting option

## 🔧 Configuration

### Tailwind CSS
Customize the design system in `tailwind.config.js`:

```javascript
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: 'your-primary-color',
        // ... other custom colors
      }
    }
  }
}
```

### API Configuration
Configure API endpoints in `src/lib/api-client.ts`:

```typescript
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 10000,
});
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Development Setup
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

### Code Style
- Use TypeScript for all new code
- Follow the existing component patterns
- Use Tailwind CSS for styling
- Write meaningful commit messages

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Documentation**: Check the docs folder for detailed guides
- **Issues**: Report bugs and request features via GitHub Issues
- **Discussions**: Join community discussions on GitHub Discussions

## 🙏 Acknowledgments

- Built with [React](https://reactjs.org/)
- Styled with [Tailwind CSS](https://tailwindcss.com/)
- Powered by [Vite](https://vitejs.dev/)
- Icons from [Lucide](https://lucide.dev/)

---

**Queue Management Admin Dashboard** - Professional queue management made simple.
