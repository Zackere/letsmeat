import { MaterialCommunityIcons } from '@expo/vector-icons';
import React from 'react';
import { Text, TouchableOpacity } from 'react-native';
import {
  Appbar
} from 'react-native-paper';

const Header = ({
  scene, previous, navigation
}) => {
  const { options } = scene.descriptor;
  const title = options.headerTitle !== undefined ? options.headerTitle
    : options.title !== undefined
      ? options.title
      : scene.route.name;

  return (
    <Appbar.Header>
      {previous ? (
        <Appbar.BackAction
          onPress={navigation.goBack}
        />
      ) : (
        <Appbar.Action onPress={() => navigation.openDrawer()} icon="menu" />
      )}
      <Appbar.Content
        title={title}
      />
      { options.rightAction
      && <Appbar.Action icon={options.rightIcon} onPress={options.rightAction} />}
    </Appbar.Header>
  );
};

export { Header };
