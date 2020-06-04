import * as React from 'react';
import Social from './Social';
import Logo from './Logo';
import { css } from '@emotion/core';

export default () => (
  <div className="d-flex bg-smoke justify-content-around align-items-center flex-wrap flex-sm-nowrap py-4">
    <Logo size="3rem" />

    <div className="text-muted text-titillium">
      <div className="d-flex mx-auto justify-content-center">
        <div className="d-flex">
          © 2019-{new Date().getFullYear()} Nathaniel's Dev Area. All rights reserved.
        </div>
      </div>
      <div className="d-flex mx-auto justify-content-center">
        Made by Nathaniel with
        <span
          role="img"
          aria-label="love"
          css={css`
            margin-left: 0.3rem;
          `}
        >
          ❤️
        </span>
        and effort
      </div>
    </div>

    <div className="mt-2 mt-sm-auto">
      <Social />
    </div>
  </div>
);
