import React, { useRef } from 'react';
import Bricks from 'bricks.js';

export default function Masonry() {
  const masonryContainer = useRef(null);

  return <div ref={masonryContainer}></div>;
}
