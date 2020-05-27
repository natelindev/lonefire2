const emotionBabelPreset = require('@emotion/babel-preset-css-prop').default(undefined, {});

module.exports = {
  babel: {
    plugins: [...emotionBabelPreset.plugins],
  },
  plugins: [
    {
      plugin: require('craco-plugin-scoped-css'),
    },
  ],
};
