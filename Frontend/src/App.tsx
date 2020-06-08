import * as React from 'react';
import 'bootstrap-css-only/css/bootstrap.min.css';
import 'material-design-icons-iconfont/dist/material-design-icons.css';
import '@fortawesome/fontawesome-free/css/all.min.css';
import './components/animation.scss';
import './App.scss';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './view/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';

const App: React.SFC = () => (
  <Layout>
    <Route exact path="/" component={Home} />
    <Route path="/counter" component={Counter} />
    <Route path="/fetch-data/:startDateIndex?" component={FetchData} />
  </Layout>
);

export default App;
