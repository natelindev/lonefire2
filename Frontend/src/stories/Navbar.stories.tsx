import React from 'react';
import Navbar from '../components/Navbar';
import { BrowserRouter as Router } from 'react-router-dom';
import Logo from '../components/Logo';
import 'bootstrap-css-only/css/bootstrap.min.css';
import SearchBar from '../components/SearchBar';
export default { title: 'Navbar' };

export const normal = () => (
  <Router>
    <Navbar />
  </Router>
);

export const logo = () => <Logo />;

export const searchBar = () => <SearchBar />;
