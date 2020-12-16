import React, { useContext } from 'react';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { createStackNavigator } from '@react-navigation/stack';
import { ScrollView, StyleSheet } from 'react-native';
import {
  Button, Card, Dialog, Paragraph, Portal, Surface
} from 'react-native-paper';
import { deleteGroup } from '../../Requests';
import { store } from '../../Store';
import { Header } from '../Header';
import Invite from './invite';

const DeleteGroup = ({ onPress }) => (
  <Card
    style={styles.delete}
    // elevation={3}
    onPress={onPress}
  >
    <Card.Content>
      <MaterialCommunityIcons
        name="delete"
        size={20}
        // color={color}
      />
      <Paragraph> DELETE </Paragraph>
    </Card.Content>
  </Card>
);

const GroupMembers = ({ members, navigation }) => (
  <Card
    style={styles.emptyCard}
    // elevation={3}
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

  const [visible, setVisible] = React.useState(false);

  const showDialog = () => setVisible(true);

  const hideDialog = () => setVisible(false);

  return (
    <Surface style={styles.groupsContainer}>
      <ScrollView>
        <GroupMembers members={state.group.users} navigation={navigation}/>
        <DeleteGroup onPress={
          showDialog
        }
        />
      </ScrollView>
      <Portal>
        <Dialog visible={visible} onDismiss={hideDialog}>
          <Dialog.Title>Warning</Dialog.Title>
          <Dialog.Content>
            <Paragraph>Are you sure you want to delete the group?</Paragraph>
          </Dialog.Content>
          <Dialog.Actions>
            <Button onPress={hideDialog}>Abort</Button>
            <Button onPress={() => {
              deleteGroup({ state }, state.group.id);
              hideDialog();
              navigation.navigate('SelectGroup');
            }}
            >
              Delete
            </Button>
          </Dialog.Actions>
        </Dialog>
      </Portal>
    </Surface>
  );
};

const Stack = createStackNavigator();

const Settings = () => {
  const { state, dispatch } = useContext(store);

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
  delete: {
    margin: 25,
    height: 50,
    backgroundColor: 'red'
  }
});

export default Settings;
export { Settings };
