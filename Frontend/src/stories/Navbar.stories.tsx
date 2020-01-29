import React from 'react';
import 'bootstrap-css-only/css/bootstrap.min.css';
import 'material-design-icons/iconfont/material-icons.css';
import '@fortawesome/fontawesome-free/css/all.min.css';

import Navbar from '../components/Navbar';
import { BrowserRouter as Router } from 'react-router-dom';
import Logo from '../components/Logo';
import SearchBar from '../components/SearchBar';

export default { title: 'Navbar' };

export const navbar = () => (
  <Router>
    <Navbar />
  </Router>
);

export const logo = () => <Logo />;

export const searchBar = () => <SearchBar />;
