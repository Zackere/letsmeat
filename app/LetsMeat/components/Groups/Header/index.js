import React, { useEffect } from 'react';
import { TouchableOpacity, Text, RefreshControlBase } from 'react-native';
import { createStackNavigator } from '@react-navigation/stack';
import {
  Appbar, Avatar, DarkTheme, useTheme, DefaultTheme
} from 'react-native-paper';
import { MaterialCommunityIcons } from '@expo/vector-icons';

const Header = ({ scene, previous, navigation }) => {
  useEffect(() => console.log(DefaultTheme), []);

  console.log(navigation.pop)
  console.log(navigation)
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
          <MaterialCommunityIcons name="dots-horizontal" size={40} />
        </TouchableOpacity>
      )}
      <Appbar.Content
        title={<Text>{title}</Text>}
      />
      {/* <Appbar.Content
        title={
          previous ? title : <MaterialCommunityIcons name="twitter" size={40} />
        }
      /> */}
    </Appbar.Header>
  );
};

export { Header };