import * as React from 'react';
import Navbar from './Navbar';
import Footbar from './Footbar';
import './Layout.scss';
import ScrollToTop from './ScrollToTop';

export default (props: { children?: React.ReactNode }) => (
  <React.Fragment>
    <header>
      <Navbar />
    </header>
    <main>
      {props.children}
      <ScrollToTop />
    </main>
    <footer>
      <Footbar />
    </footer>
  </React.Fragment>
);
