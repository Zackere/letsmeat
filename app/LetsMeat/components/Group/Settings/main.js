import { MaterialCommunityIcons } from '@expo/vector-icons';
import React, { useContext, useState } from 'react';
import { ScrollView, StyleSheet } from 'react-native';
import {
  Button, Card, Dialog, Paragraph, Portal, Surface
} from 'react-native-paper';
import { deleteGroup, leaveGroup } from '../../Requests';
import { store } from '../../Store';
import GroupMembers from './members';

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

const SettingsScroll = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

  return (
    <Surface style={styles.groupsContainer}>
      <ScrollView>
        <GroupMembers members={state.group.users} navigation={navigation} />
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
      </ScrollView>
    </Surface>
  );
};

const styles = StyleSheet.create({
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

export default SettingsScroll;
