import React from 'react';
import { useScrollPosition } from '../hooks/scrollPosition';
import './ScrollToTop.scoped.scss';

export default () => {
  const scrollPosition = useScrollPosition();
  return (
    <div
      className={`scroll-to-top rounded z-1 ${scrollPosition > 100 ? 'show' : 'hidden'}`}
      onClick={() => {
        window.scroll({
          top: 0,
          left: 0,
          behavior: 'smooth',
        });
      }}
    >
      <i className="fas fa-angle-up" />
    </div>
  );
};
