import { MaterialCommunityIcons } from '@expo/vector-icons';
import React from 'react';
import { Text, TouchableOpacity } from 'react-native';
import {
  Appbar
} from 'react-native-paper';

const Header = ({ scene, previous, navigation }) => {

  const { options } = scene.descriptor;
  const title = options.headerTitle !== undefined ? options.headerTitle
    : options.title !== undefined
      ? options.title
      : scene.route.name;

  return (
    <Appbar.Header
    >
      {previous ? (
        <Appbar.BackAction
          onPress={navigation.goBack}
        />
      ) : (
        <TouchableOpacity
          onPress={() => {
            navigation.openDrawer();
          }}
        >
          <MaterialCommunityIcons name="menu" size={30} />
        </TouchableOpacity>
      )}
      <Appbar.Content
        title={<Text>{title}</Text>}
      />
    </Appbar.Header>
  );
};

export { Header };
