import React, { useContext } from 'react';
import { View, StyleSheet } from 'react-native';
import {
  Button, Caption,
  IconButton, Subheading, Surface
} from 'react-native-paper';
import { getNotificationTimestamp, refreshNotifications } from '../../helpers/notifications';
import { store } from '../Store';
import { Notification } from './common';

const Notifications = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

  const notifications = [...state.invitations, ...state.debts]
    .sort((a, b) => getNotificationTimestamp(b) - getNotificationTimestamp(a));

  return (
    <Surface style={styles.container}>
      {notifications.length > 0
        ? (
          <>
            {notifications
              .map((item) => (
                <Notification
                  item={item}
                  key={`${item.group_id}${item.id}`}
                />
              ))}
          </>
        )
        : (
          <View style={{ alignItems: 'center', marginBottom: 10 }}>
            <Caption>You have no pending notifications</Caption>
          </View>
        )}
    </Surface>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  }
});

export default Notifications;
