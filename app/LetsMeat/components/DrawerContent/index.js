import React, { useContext } from 'react';
import { View, StyleSheet, Text } from 'react-native';
import { DrawerItem, DrawerContentScrollView } from '@react-navigation/drawer';
import {
  Drawer,
  Title,
  Caption,
  Avatar,
  Paragraph,
  TouchableRipple,
  Switch,
} from 'react-native-paper';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { store } from '../Store';

function DrawerContent(props) {
  const { state, dispatch } = useContext(store);
  console.log(state.user);
  return (
    <DrawerContentScrollView {...props}>
      <View style={styles.drawerContent}>
        <View style={styles.userInfo}>
          <Avatar.Image size={50} source={{uri: state.user.photo}} />
          <Title style={styles.title}>{state.user.name}</Title>
          <Caption style={styles.caption}>{state.user.email}</Caption>
        </View>
      </View>
      <Drawer.Section style={styles.drawerSection}>
        <DrawerItem
          icon={({ color, size }) => (
            <MaterialCommunityIcons
              name="account-group-outline"
              color={color}
              size={size}
            />
          )}
          label="Groups"
        />
      </Drawer.Section>

    </DrawerContentScrollView>
  );
}

const styles = StyleSheet.create({
  drawerContent: {
    flex: 1,
  },
  userInfo: {
    paddingLeft: 20,
    paddingTop: 20
  },
  caption: {
    fontSize: 14,
    lineHeight: 14,
    color: 'grey'
  },
  title: {
    marginTop: 20,
    fontWeight: 'bold',
    color: 'black'
  },
  drawerSection: {
    marginTop: 15,
  },
});

export default DrawerContent;
