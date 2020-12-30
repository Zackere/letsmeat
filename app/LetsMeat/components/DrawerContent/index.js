import { MaterialCommunityIcons } from '@expo/vector-icons';
import {
  GoogleSignin
} from '@react-native-community/google-signin';
import { DrawerItem } from '@react-navigation/drawer';
import React, { useContext } from 'react';
import {
  StyleSheet, View
} from 'react-native';
import {
  Avatar,
  Caption, Drawer,
  Title
} from 'react-native-paper';
import { store } from '../Store';
import Notifications from '../Notifications/drawer';

export const DrawerButton = ({
  onPress, icon, label
}) => {
  const x = 0;
  return (
    <DrawerItem
      icon={({ color, size }) => (
        <MaterialCommunityIcons
          name={icon}
          color={color}
          size={size}
        />
      )}
      onPress={onPress}
      label={label}
    />
  );
};

function DrawerContent({ navigation }) {
  const { state, dispatch } = useContext(store);

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
        <DrawerButton
          icon="settings-outline"
          label="Preferences"
          onPress={() => navigation.navigate('Preferences')}
        />
      </Drawer.Section>
      <Drawer.Section style={styles.drawerSection}>
        {state.group && state.group.id && (
        <DrawerButton
          icon="account-multiple-outline"
          label="Current Group"
          onPress={() => navigation.navigate('Feed')}
        />
        )}
        <DrawerButton
          icon="account-group-outline"
          label="Groups"
          onPress={() => navigation.navigate('Groups')}
        />
      </Drawer.Section>
      <Drawer.Section style={{ ...styles.drawerSection, ...styles.notificationsSection }}>
        <Notifications navigation={navigation} />
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
  notificationsSection: {
    marginTop: 5
  },
  lastDrawerSection: {
    position: 'absolute',
    bottom: 0,
    width: '100%',
    marginBottom: 15
  }
});

export default DrawerContent;
