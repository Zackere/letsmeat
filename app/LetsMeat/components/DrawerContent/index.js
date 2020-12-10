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
  BottomNavigation,
  Modal,
  Provider,
  Portal
} from 'react-native-paper';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { color } from 'react-native-reanimated';
import { store } from '../Store';
import {
  GoogleSigninButton,
  GoogleSignin,
  statusCodes
} from '@react-native-community/google-signin';

function DrawerContent({ navigation }) {
  const { state, dispatch } = useContext(store);
  // console.log(navigation)
  return (
    <View
      style={styles.drawer}
    >
      <View>
        <View style={styles.userInfo}>
          <Avatar.Image size={50} source={{ uri: state.user.photo }} />
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
          onPress={() => {
            navigation.navigate('Groups');
          }}
          label="Groups"
        />
      </Drawer.Section>
      <Drawer.Section style={styles.lastDrawerSection}>
        <DrawerItem
          icon={({ color, size }) => (
            <MaterialCommunityIcons
              name="logout"
              color={color}
              size={size}
            />
          )}
          label="Log Out"
          onPress={async () => {
            try {
              // await GoogleSignin.revokeAccess();
              await GoogleSignin.signOut();
              dispatch({ type: 'LOGOUT' });
            } catch (error) {
              console.error(error);
            }
          }}
        />
      </Drawer.Section>
    </View>
  );
}

const styles = StyleSheet.create({
  drawer: {
    display: 'flex',
    flexDirection: 'column',
    height: '100%',
    justifyContent: 'flex-start',
    alignContent: 'flex-start'
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
  lastDrawerSection: {
    position: 'absolute',
    bottom: 0,
    width: '100%',
    marginBottom: 15
  }
});

export default DrawerContent;
