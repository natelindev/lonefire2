import * as React from 'react';
import Social from './Social';
import Logo from './Logo';
import './Footbar.scoped.scss';

export default () => (
  <div className="d-flex bg-smoke justify-content-around align-items-center flex-wrap flex-sm-nowrap py-4">
    <Logo size="3rem" />

    <div className="text-muted text-titillium">
      <div className="d-flex mx-auto justify-content-center">
        Made by Nathaniel with
        <span id="love" role="img" aria-label="love">
          ❤️
        </span>
        and effort
      </div>
      <div className="d-flex mx-auto justify-content-center">
        <a className="animated--link">Privacy Policy</a>
      </div>
    </div>

    <div className="mt-2 mt-sm-auto">
      <Social />
    </div>
  </div>
);
