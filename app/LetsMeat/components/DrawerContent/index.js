import React from 'react';
import {View, StyleSheet, Text} from 'react-native';
import {DrawerItem, DrawerContentScrollView} from '@react-navigation/drawer';
import {
  Drawer,
  Title,
  Caption,
  Paragraph,
  TouchableRipple,
  Switch,
} from 'react-native-paper';
import {MaterialCommunityIcons} from '@expo/vector-icons';

function DrawerContent(props) {
  // console.log(props);
  return (
    <DrawerContentScrollView {...props}>
      <View style={styles.drawerContent}>
        <View style={styles.userInfo}>
          <Title style={styles.title}>Jan Kowalski</Title>
          <Caption style={styles.caption}>jan.kowalski@gmail.com</Caption>
        </View>
      </View>
      <Drawer.Section style={styles.drawerSection}>
        <DrawerItem
          icon={({color, size}) => (
            <MaterialCommunityIcons
              name="account-group-outline"
              color={color}
              size={size}
            />
          )}
          label="Groups"
        />
      </Drawer.Section>
    </DrawerContentScrollView>
  );
}

const styles = StyleSheet.create({
  drawerContent: {
    flex: 1,
  },
  userInfo: {
    paddingLeft: 20,
  },
  caption: {
    fontSize: 14,
    lineHeight: 14,
  },
  title: {
    marginTop: 20,
    fontWeight: 'bold',
  },
  drawerSection: {
    marginTop: 15,
  },
});

export default DrawerContent;
