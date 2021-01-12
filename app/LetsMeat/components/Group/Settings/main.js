import React, { useContext } from 'react';
import { ScrollView, StyleSheet, RefreshControl } from 'react-native';
import BackgroundContainer, { ScrollPlaceholder } from '../../Background';
import ModalButton from '../../Buttons';
import {
  deleteGroup, leaveGroup, getGroupInfo, getGroupDebts
} from '../../Requests';
import { store } from '../../Store';
import { GroupMembers } from './members';

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

const SettingsScroll = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const [refreshing, setRefreshing] = React.useState(false);

  const onRefresh = () => {
    setRefreshing(true);
    Promise.all(
      [getGroupInfo({ state, dispatch }, state.group.id),
        getGroupDebts({ state, dispatch }, state.group.id)]
    )
      .then(([groupInfo, debtInfo]) => {
        setRefreshing(false);
        dispatch({ type: 'SET_GROUP', payload: { ...groupInfo, ...debtInfo } });
      });
  };

  return (
    <BackgroundContainer>
      <ScrollView
        refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
      >
        <GroupMembers members={state.group.users} debts={state.group.debts} navigation={navigation} />
        <LeaveGroup confirmAction={() => {
          leaveGroup({ state, dispatch }, state.group.id)
            .then(() => dispatch({ type: 'REMOVE_GROUP', groupId: state.group.id }))
            .then(() => dispatch({ type: 'SET_GROUP', payload: {} }))
            .then(() => navigation.navigate('SelectGroup'));
        }}
        />
        <DeleteGroup confirmAction={() => {
          deleteGroup({ state, dispatch }, state.group.id)
            .then(() => dispatch({ type: 'REMOVE_GROUP', groupId: state.group.id }))
            .then(() => navigation.navigate('SelectGroup'));
        }}
        />
        <ScrollPlaceholder height={100} />
      </ScrollView>
    </BackgroundContainer>
  );
};

const styles = StyleSheet.create({
  groupsContainer: {
    width: '100%',
    height: '100%',
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
    backgroundColor: 'rgba(255, 0, 0, 0.8)'
  },
  leave: {
    backgroundColor: '#fc3503'
  }
});

export default SettingsScroll;
