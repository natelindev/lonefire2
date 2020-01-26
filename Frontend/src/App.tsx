import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';

import 'bootstrap-css-only/css/bootstrap.min.css';
import 'material-design-icons/iconfont/material-icons.css';
import '@fortawesome/fontawesome-free/css/all.min.css';

export default () => (
  <Layout>
    <Route exact path="/" component={Home} />
    <Route path="/counter" component={Counter} />
    <Route path="/fetch-data/:startDateIndex?" component={FetchData} />
  </Layout>
);
