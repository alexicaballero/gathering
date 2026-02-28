import type { Metadata } from 'next';
import { Geist, Geist_Mono } from 'next/font/google';
import './globals.css';
import SiteHeader from '@/components/site-header';

const geistSans = Geist({
  variable: '--font-geist-sans',
  subsets: ['latin'],
});

const geistMono = Geist_Mono({
  variable: '--font-geist-mono',
  subsets: ['latin'],
});

export const metadata: Metadata = {
  title: 'Gathering - Communities of Practice',
  description:
    'Manage and explore sessions from communities of practice in Frontend, Python and Cloud.',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang='en'>
      <body
        className={`${geistSans.variable} ${geistMono.variable} font-sans antialiased`}
      >
        <div className='flex min-h-svh flex-col'>
          <SiteHeader />
          <main className='flex-1'>{children}</main>
          {/* <SiteFooter /> */}
        </div>
        {/* <Toaster /> */}
      </body>
    </html>
  );
}
