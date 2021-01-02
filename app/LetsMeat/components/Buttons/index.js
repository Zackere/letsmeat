import { MaterialCommunityIcons } from '@expo/vector-icons';
import React, { useState } from 'react';
import {
  Button, Card, Dialog, Paragraph, Portal
} from 'react-native-paper';
import { StyleSheet } from 'react-native';

export const ModalButton = ({
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
    backgroundColor: 'rgba(255, 255, 255, 0.9)'
  },
  delete: {
    backgroundColor: 'rgba(255, 128, 128, 0.9)'
  },
  leave: {
    backgroundColor: '#fc3503'
  }
});

export default ModalButton;
