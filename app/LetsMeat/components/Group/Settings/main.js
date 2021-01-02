import React, { useContext } from 'react';
import { ScrollView, StyleSheet } from 'react-native';
import BackgroundContainer, { ScrollPlaceholder } from '../../Background';
import ModalButton from '../../Buttons';
import { deleteGroup, leaveGroup } from '../../Requests';
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

  return (
    <BackgroundContainer>
      <ScrollView>
        <GroupMembers members={state.group.users} debts={state.group.debts} navigation={navigation} />
        <LeaveGroup confirmAction={() => {
          leaveGroup({ state }, state.group.id)
            .then(() => dispatch({ type: 'REMOVE_GROUP', groupId: state.group.id }))
            .then(() => dispatch({ type: 'SET_GROUP', payload: {} }))
            .then(() => navigation.navigate('SelectGroup'));
        }}
        />
        <DeleteGroup confirmAction={() => {
          deleteGroup({ state }, state.group.id)
            .then(() => dispatch({ type: 'REMOVE_GROUP', groupId: state.group.id }))
            .then(() => navigation.navigate('SelectGroup'));
        }}
        />
        <ScrollPlaceholder />
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
