import React, { useContext } from 'react';
import { StyleSheet, View } from 'react-native';
import {
  Caption,
  Surface
} from 'react-native-paper';
import { getNotificationTimestamp } from '../../helpers/notifications';
import BackgroundContainer from '../Background';
import { store } from '../Store';
import { Notification } from './common';

const Notifications = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

  const notifications = [...state.invitations, ...state.debts]
    .sort((a, b) => getNotificationTimestamp(b) - getNotificationTimestamp(a));

  return (
    <BackgroundContainer backgroundVariant={3}>
      {notifications.length > 0
        ? (
          <>
            {notifications
              .map((item) => (
                <Notification
                  item={item}
                  key={`${item.group_id}${item.id}`}
                  full
                />
              ))}
          </>
        )
        : (
          <View style={{ alignItems: 'center', marginBottom: 10 }}>
            <Caption>You have no pending notifications</Caption>
          </View>
        )}
    </BackgroundContainer>
  );
};

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: '100%'
  }
});

export default Notifications;
