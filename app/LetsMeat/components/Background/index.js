import React from 'react';
import { ImageBackground, View, Dimensions } from 'react-native';
import { Surface } from 'react-native-paper';

export const BackgroundContainer = ({ backgroundVariant = 'food', children }) => {
  let source;
  switch (backgroundVariant) {
    case 'food':
      source = require('../../images/background1.jpg');
      break;
    case 'office':
      source = require('../../images/background2.jpg');
      break;
    case 'money':
      source = require('../../images/background3.jpg');
      break;
    case 'settings':
      source = require('../../images/background4.jpg');
      break;
    case 'searching':
      source = require('../../images/background5.jpg');
      break;
    case 'fireworks':
      source = require('../../images/background6.jpg');
      break;
    default:
      source = require('../../images/background1.jpg');
  }
  return (
    <ImageBackground
      style={{
        position: 'absolute',
        left: 0,
        top: 0,
        width: Dimensions.get('window').width,
        height: Dimensions.get('window').height,
      }}
      source={source}
    >
      <Surface style={{ width: '100%', height: '100%', backgroundColor: 'rgba(200, 200, 200, 0.3)' }}>
        {children}
      </Surface>
    </ImageBackground>
  );
};

export const ScrollPlaceholder = ({ height = 50 }) => (
  <View style={{ height, backgroundColor: 'rgba(0, 0, 0, 0)' }} />
);

export default BackgroundContainer;
