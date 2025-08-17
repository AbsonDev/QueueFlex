import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '@/stores/auth';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/Card';
import { Badge } from '@/components/ui/Badge';
import { Building2, Users, Clock, BarChart3 } from 'lucide-react';

const loginSchema = z.object({
  email: z.string().email('Please enter a valid email address'),
  password: z.string().min(1, 'Password is required'),
  tenant: z.string().optional(),
});

type LoginFormData = z.infer<typeof loginSchema>;

export const Login: React.FC = () => {
  const navigate = useNavigate();
  const { login, isLoading, error } = useAuthStore();
  const [showTenantField, setShowTenantField] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      await login(data);
      navigate('/dashboard');
    } catch (error) {
      // Error is handled by the store
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
      <div className="w-full max-w-md space-y-8">
        {/* Header */}
        <div className="text-center">
          <div className="mx-auto h-16 w-16 bg-primary rounded-full flex items-center justify-center mb-4">
            <Building2 className="h-8 w-8 text-white" />
          </div>
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            Queue Management
          </h1>
          <p className="text-gray-600">
            Admin Dashboard
          </p>
        </div>

        {/* Login Form */}
        <Card className="shadow-xl">
          <CardHeader className="text-center">
            <CardTitle>Sign in to your account</CardTitle>
            <CardDescription>
              Enter your credentials to access the dashboard
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <Input
                label="Email"
                type="email"
                placeholder="Enter your email"
                error={errors.email?.message}
                {...register('email')}
              />

              <Input
                label="Password"
                type="password"
                placeholder="Enter your password"
                error={errors.password?.message}
                {...register('password')}
              />

              {showTenantField && (
                <Input
                  label="Tenant (Optional)"
                  type="text"
                  placeholder="Enter tenant subdomain"
                  helperText="Leave empty for default tenant"
                  {...register('tenant')}
                />
              )}

              <div className="flex items-center justify-between">
                <button
                  type="button"
                  onClick={() => setShowTenantField(!showTenantField)}
                  className="text-sm text-primary hover:underline"
                >
                  {showTenantField ? 'Hide' : 'Show'} tenant field
                </button>
              </div>

              {error && (
                <div className="p-3 bg-red-50 border border-red-200 rounded-md">
                  <p className="text-sm text-red-600">{error}</p>
                </div>
              )}

              <Button
                type="submit"
                className="w-full"
                loading={isLoading}
                disabled={isLoading}
              >
                {isLoading ? 'Signing in...' : 'Sign in'}
              </Button>
            </form>
          </CardContent>
        </Card>

        {/* Features */}
        <div className="grid grid-cols-2 gap-4">
          <div className="text-center p-4 bg-white rounded-lg shadow-sm">
            <Users className="h-6 w-6 text-blue-600 mx-auto mb-2" />
            <p className="text-sm font-medium text-gray-900">User Management</p>
            <p className="text-xs text-gray-500">Manage users and roles</p>
          </div>
          <div className="text-center p-4 bg-white rounded-lg shadow-sm">
            <Clock className="h-6 w-6 text-green-600 mx-auto mb-2" />
            <p className="text-sm font-medium text-gray-900">Queue Control</p>
            <p className="text-xs text-gray-500">Real-time queue management</p>
          </div>
          <div className="text-center p-4 bg-white rounded-lg shadow-sm">
            <BarChart3 className="h-6 w-6 text-purple-600 mx-auto mb-2" />
            <p className="text-sm font-medium text-gray-900">Analytics</p>
            <p className="text-xs text-gray-500">Performance insights</p>
          </div>
          <div className="text-center p-4 bg-white rounded-lg shadow-sm">
            <Badge variant="success" className="mx-auto mb-2">
              Open Source
            </Badge>
            <p className="text-xs text-gray-500">MIT Licensed</p>
          </div>
        </div>

        {/* Footer */}
        <div className="text-center text-sm text-gray-500">
          <p>© 2024 Queue Management System</p>
          <p>Built with React, TypeScript & Tailwind CSS</p>
        </div>
      </div>
    </div>
  );
};