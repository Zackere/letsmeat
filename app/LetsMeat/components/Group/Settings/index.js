import { getFocusedRouteNameFromRoute } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import React, { useContext } from 'react';
import { StyleSheet } from 'react-native';
import { Header } from '../../Header';
import { store } from '../../Store';
import Invite from './invite';
import SettingsScroll from './main';
import SendTransfer from './sendTransfer';
import { MembersScreen } from './members';

const Stack = createStackNavigator();

const SCREENS_WITHOUT_TABS = new Set(['Invite', 'Members', 'SendTransfer']);

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
      screenOptions={{
        headerTitle: state.group.name,
        header: ({ scene, previous, navigation }) => (
          <Header scene={scene} previous={previous} navigation={navigation} />
        ),
      }}
    >
      <Stack.Screen
        name="Settings"
        component={SettingsScroll}
      />
      <Stack.Screen
        name="Invite"
        component={Invite}
      />
      <Stack.Screen
        name="SendTransfer"
        component={SendTransfer}
      />
      <Stack.Screen
        name="Members"
        component={MembersScreen}
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
