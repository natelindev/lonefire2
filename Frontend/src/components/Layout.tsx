import * as React from 'react';
import Navbar from './Navbar';
import Footbar from './Footbar';
import './Layout.scss';
import ScrollToTop from './ScrollToTop';

interface LayoutProps {
  children?: React.ReactNode;
}

const Layout: React.SFC<LayoutProps> = (props: LayoutProps) => {
  const { children } = props;
  return (
    <>
      <header>
        <Navbar />
      </header>
      <main>
        {children}
        <ScrollToTop />
      </main>
      <footer>
        <Footbar />
      </footer>
    </>
  );
};
export default Layout;
