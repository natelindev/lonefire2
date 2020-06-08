import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import Navbar from '../components/Navbar';
import Logo from '../components/Logo';
import SearchBar from '../components/SearchBar';

export default { title: 'Navbar' };

export const navbar = (): React.ReactElement => (
  <Router>
    <Navbar />
  </Router>
);

export const logo = (): React.ReactElement => <Logo />;

export const searchBar = (): React.ReactElement => <SearchBar />;
