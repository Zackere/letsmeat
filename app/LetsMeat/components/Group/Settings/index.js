import { MaterialCommunityIcons } from '@expo/vector-icons';
import { createStackNavigator } from '@react-navigation/stack';
import React, { useContext, useState } from 'react';
import { ScrollView, StyleSheet } from 'react-native';
import { getFocusedRouteNameFromRoute } from '@react-navigation/native';
import {
  Button, Card, Dialog, Paragraph, Portal, Surface
} from 'react-native-paper';
import { deleteGroup, leaveGroup } from '../../Requests';
import { store } from '../../Store';
import { Header } from '../../Header';
import Invite from './invite';

const ModalButton = ({
  style, modalText, confirmAction, confirmText, icon, buttonText
}) => {
  const [visible, setVisible] = useState(false);
  const showDialog = () => setVisible(true);
  const hideDialog = () => setVisible(false);

  return (
    <>
      <Card
        style={{ ...styles.cardButton, ...style }}
        onPress={showDialog}
      >
        <Card.Content style={{ flexDirection: 'row', height: '100%', alignItems: 'center' }}>
          <MaterialCommunityIcons
            name={icon}
            size={20}
          />
          <Paragraph>
            {buttonText}
          </Paragraph>
        </Card.Content>
      </Card>
      <Portal>
        <Dialog visible={visible} onDismiss={hideDialog}>
          <Dialog.Title>Warning</Dialog.Title>
          <Dialog.Content>
            <Paragraph>{modalText}</Paragraph>
          </Dialog.Content>
          <Dialog.Actions>
            <Button onPress={hideDialog}>Abort</Button>
            <Button onPress={() => {
              confirmAction();
              hideDialog();
            }}
            >
              {confirmText}
            </Button>
          </Dialog.Actions>
        </Dialog>
      </Portal>
    </>
  );
};

const DeleteGroup = ({ confirmAction }) => (
  <ModalButton
    style={styles.delete}
    modalText="Are you sure you want to delete the group?"
    icon="delete"
    buttonText="DELETE"
    confirmAction={confirmAction}
    confirmText="Delete"
  />
);

const LeaveGroup = ({ confirmAction }) => (
  <ModalButton
    modalText="Are you sure you want to leave the group?"
    icon="logout-variant"
    buttonText="LEAVE"
    confirmAction={confirmAction}
    confirmText="Leave"
  />
);
const GroupMembers = ({ members, navigation }) => (
  <Card
    style={styles.emptyCard}
  >
    <Card.Content>
      {members.map((m) => (
        <Card
          key={m.id}
          style={styles.user}
        >
          <Card.Content>
            <Paragraph>
              {' '}
              {m.name}
              {' '}
            </Paragraph>
          </Card.Content>
        </Card>
      ))}
    </Card.Content>
    <Card.Actions>
      <Button onPress={() => navigation.navigate('Invite')}>Invite new members</Button>
      <Button>See all members</Button>
    </Card.Actions>
  </Card>
);

const SettingsScroll = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

  return (
    <Surface style={styles.groupsContainer}>
      <ScrollView>
        <GroupMembers members={state.group.users} navigation={navigation} />
        <LeaveGroup confirmAction={() => {
          leaveGroup({ state }, state.group.id)
            .then(() => dispatch({ type: 'REMOVE_GROUP', groupId: state.group.id }))
            .then(() => navigation.navigate('SelectGroup'));
        }}
        />
        <DeleteGroup confirmAction={() => {
          deleteGroup({ state }, state.group.id)
            .then(() => dispatch({ type: 'REMOVE_GROUP', groupId: state.group.id }))
            .then(() => navigation.navigate('SelectGroup'));
        }}
        />
      </ScrollView>
    </Surface>
  );
};

const Stack = createStackNavigator();

const SCREENS_WITHOUT_TABS = new Set(['Invite']);

const Settings = ({ navigation, route }) => {
  const { state } = useContext(store);

  React.useLayoutEffect(() => {
    const screenName = getFocusedRouteNameFromRoute(route);
    if (SCREENS_WITHOUT_TABS.has(screenName)) navigation.setOptions({ tabBarVisible: false });
    else navigation.setOptions({ tabBarVisible: true });
  }, [navigation, route]);

  return (

    <Stack.Navigator
      initialRouteName="Settings"
      headerMode="float"
      headerTitle="XD"
      screenOptions={{
        header: ({ scene, previous, navigation }) => (
          <Header scene={scene} previous={previous} navigation={navigation} />
        ),
      }}
    >
      <Stack.Screen
        name="Settings"
        component={SettingsScroll}
        options={{ headerTitle: state.group.name }}
      />
      <Stack.Screen
        name="Invite"
        component={Invite}
        options={{ headerTitle: state.group.name }}
      />

    </Stack.Navigator>
  );
};

const styles = StyleSheet.create({
  progressBar: {
    width: '100%',
    height: '100%',
    justifyContent: 'center',
    alignItems: 'center'
  },
  groupsContainer: {
    width: '100%',
    height: '100%'
  },
  fab: {
    position: 'absolute',
    margin: 30,
    right: 0,
    bottom: 0,
  },
  emptyCard: {
    margin: 25
  },
  user: {
    margin: 5
  },
  cardButton: {
    margin: 25,
    height: 50,
  },
  delete: {
    backgroundColor: '#fc3503'
  },
  leave: {
    backgroundColor: '#fc3503'
  }
});

export default Settings;
export { Settings };
