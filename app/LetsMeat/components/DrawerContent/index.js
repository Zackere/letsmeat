import { MaterialCommunityIcons } from '@expo/vector-icons';
import {
  GoogleSignin
} from '@react-native-community/google-signin';
import { DrawerItem } from '@react-navigation/drawer';
import React, { useContext, useEffect, useState } from 'react';
import {
  StyleSheet, Text, View
} from 'react-native';
import {
  ActivityIndicator, Avatar,
  Button, Caption, Card, Drawer,
  IconButton, Paragraph, Subheading, Title
} from 'react-native-paper';
import {
  getGroupInfo, getInvitations, getUsersInfo, rejectInvitation, acceptInvitation
} from '../Requests';
import { store } from '../Store';

const DrawerButton = ({
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

const InvitationButton = ({ invitation }) => {
  const { state, dispatch } = useContext(store);
  const [user, setUser] = useState(null);
  const [group, setGroup] = useState(null);

  useEffect(() => {
    getUsersInfo({ state }, invitation.from_id).then((users) => setUser(users[0]));
    getGroupInfo({ state }, invitation.group_id).then(setGroup);
  }, []);

  return (
    <Card style={{ margin: 5 }}>
      <Card.Content>
        { (user && group)
          ? (
            <Paragraph>
              <Subheading>{user.name}</Subheading>
              {'\t'}
              <Caption>invites you to join</Caption>
              {'\n'}
              <Subheading>{group.name}</Subheading>
            </Paragraph>
          )
          : <ActivityIndicator />}
      </Card.Content>
      <Card.Actions>
        <Button
          color="red"
          onPress={() => {
            rejectInvitation({ state }, group.id).then(() => dispatch({ type: 'REMOVE_INVITATION', groupId: group.id }));
          }}
        >
          Refuse
        </Button>
        <Button onPress={() => {
          acceptInvitation({ state }, group.id).then(() => dispatch({ type: 'REMOVE_INVITATION', groupId: group.id }));
        }}
        >
          Accept
        </Button>
      </Card.Actions>
    </Card>
  );
};

const Notifications = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

  useEffect(() => {}, []);

  return (
    <View>
      <View style={{ flexDirection: 'row', margin: 5 }}>
        <Subheading style={{ margin: 10 }}>Notifications</Subheading>
        <IconButton
          icon="refresh"
          size={20}
          style={{ position: 'absolute', top: -9, right: 0 }}
          onPress={() => {
            getInvitations({ state }).then((invitations) => {
              console.log(invitations);
              dispatch({ type: 'SET_INVITATIONS', payload: invitations });
            });
          }}
        />
      </View>
      {state.invitations.length > 0
        ? (
          <>
            {state.invitations.slice(0, 2)
              .map((invitation) => (
                <InvitationButton
                  invitation={invitation}
                  key={invitation.group_id}
                />
              ))}
            {state.invitations.length > 2 && <Button>Show all notifications</Button>}
          </>
        )
        : (
          <View style={{ alignItems: 'center', marginBottom: 10 }}>
            <Caption>No notifications</Caption>
          </View>
        )}
    </View>
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
        <Notifications />
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
