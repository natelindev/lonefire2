import * as React from 'react';
import Navbar from './Navbar';
import Footbar from './Footbar';
import './Layout.scss';

export default (props: { children?: React.ReactNode }) => (
  <React.Fragment>
    <header>
      <Navbar />
    </header>
    <main id="page-top">
      {props.children}
      <a className="scroll-to-top rounded z-1" href="#page-top">
        <i className="fas fa-angle-up" />
      </a>
    </main>
    <footer>
      <Footbar />
    </footer>
  </React.Fragment>
);
