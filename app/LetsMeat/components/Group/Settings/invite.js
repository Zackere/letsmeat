import React, { useState } from 'react';
import { StyleSheet, Text } from 'react-native';
import { Searchbar, Surface } from 'react-native-paper';

const SelectedUsers = ({users, onPress}) => {
  //placeholder
  return (<Text>
    Here be users
  </Text>)
}

const Invite = ({ navigation }) => {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedUsers, setSelectedUsers] = useState([]);

  const onChangeSearch = (query) => setSearchQuery(query);

  return (
    <Surface style={styles.container}>
      <SelectedUsers users={selectedUsers} onPress={console.log}/>
      <Searchbar
        style={styles.searchbar}
        placeholder="Search"
        onChangeText={onChangeSearch}
        value={searchQuery}
      />
    </Surface>
  );
};

const styles = StyleSheet.create({
  container: {
    height: '100%',
    width: '100%'
  },
  searchbar: {
    margin: 5
  }
});

export default Invite;
