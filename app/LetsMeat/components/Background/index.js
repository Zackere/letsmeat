import React from 'react';
import { ImageBackground, View } from 'react-native';
import { Surface } from 'react-native-paper';

export const BackgroundContainer = ({ backgroundVariant = 0, children }) => (
  <ImageBackground
    style={{ width: '100%', height: '100%' }}
  // source={{ uri: 'https://images.unsplash.com/photo-1414235077428-338989a2e8c0?ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&ixlib=rb-1.2.1&auto=format&fit=crop&w=1950&q=80' }}
    source={require('../../images/background1.jpg')}
  >
    <Surface style={{ width: '100%', height: '100%', backgroundColor: 'rgba(200, 200, 200, 0.5)' }}>
      {children}
    </Surface>
  </ImageBackground>
);

export const ScrollPlaceholder = ({ height = 50 }) => (
  <View style={{ height, backgroundColor: 'rgba(0, 0, 0, 0)' }} />
);

export default BackgroundContainer;
