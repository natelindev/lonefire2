import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import WebFont from 'webfontloader';

import App from './App';

WebFont.load({
  google: {
    families: ['Titillium Web:300,400,700', 'Economica:400 700', 'sans-serif'],
  },
});

ReactDOM.render(
  <BrowserRouter>
    <App />
  </BrowserRouter>,
  document.getElementById('root')
);
