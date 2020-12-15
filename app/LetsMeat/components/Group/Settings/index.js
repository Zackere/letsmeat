import React, { useContext } from 'react';
import {
  Text, View, StyleSheet, ScrollView
} from 'react-native';
import {
  Card, Surface, Paragraph, FlatList, Portal, Dialog, Button
} from 'react-native-paper';
import { createStackNavigator } from '@react-navigation/stack';
import { TouchableHighlight } from 'react-native-gesture-handler';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { Header } from '../Header';
import { store } from '../../Store';
import { getGroupInfo, deleteGroup } from '../../Requests';

const DeleteGroup = ({ onPress }) => (
  <Card
    style={styles.delete}
    elevation={3}
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

const GroupMembers = ({ members, onPress }) => {
  console.log('x');
  return (
    <Card
      style={styles.emptyCard}
      elevation={3}
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
    </Card>
  );
};

const SettingsScroll = ({navigation}) => {
  const { state, dispatch } = useContext(store);

  const [visible, setVisible] = React.useState(false);

  const showDialog = () => setVisible(true);

  const hideDialog = () => setVisible(false);

  return (
    <Surface style={styles.groupsContainer}>
      <ScrollView>
        <GroupMembers members={state.group.users || [1, 2, 3]} />
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
              navigation.navigate('SelectGroup')
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
