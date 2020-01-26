import * as React from 'react';
import { Container } from 'reactstrap';
import Navbar from './Navbar';
import Footbar from './Footbar';
import './Layout.scss';

export default (props: { children?: React.ReactNode }) => (
  <React.Fragment>
    <header>
      <Navbar></Navbar>
    </header>
    <main id="page-top">
      {props.children}
      <a className="scroll-to-top rounded z-1" href="#page-top">
        <i className="fas fa-angle-up"></i>
      </a>
    </main>
    <footer>
      <Footbar></Footbar>
    </footer>
  </React.Fragment>
);
