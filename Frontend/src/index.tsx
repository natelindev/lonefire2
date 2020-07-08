import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import { RecoilRoot } from 'recoil';
import WebFont from 'webfontloader';

import App from './App';
import DataSubscription from './view/DataSubscription';

WebFont.load({
  google: {
    families: ['Titillium Web:300,400,700', 'Economica:400 700', 'sans-serif'],
  },
});

ReactDOM.render(
  <RecoilRoot>
    <BrowserRouter>
      <App />
    </BrowserRouter>
    <DataSubscription />
  </RecoilRoot>,
  document.getElementById('root')
);
