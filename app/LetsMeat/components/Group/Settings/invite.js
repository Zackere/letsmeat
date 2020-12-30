import React, {
  useCallback, useContext, useState, useRef,
  useEffect
} from 'react';
import { debounce } from 'debounce';
import {
  StyleSheet, Text, View, Image
} from 'react-native';
import { ScrollView } from 'react-native-gesture-handler';
import {
  Card, Chip, Searchbar, Surface, Button, Avatar, FAB
} from 'react-native-paper';
import { searchUsers, sendInvitation } from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';

const SelectedUsers = ({ users, onClose }) => (
  <View style={styles.selectedUserContainer}>
    <ScrollView horizontal>
      {users ? users.map((user) => (
        <Chip
          key={user.id}
          avatar={<Avatar.Image source={{ uri: user.picture_url }} size={24} />}
          onClose={onClose}
        >
          <Text>
            {user.name}
          </Text>
        </Chip>
      ))
        : <Chip><Text>Nothin to show</Text></Chip>}
    </ScrollView>
  </View>
);

const Invite = ({ navigation }) => {
  const { state } = useContext(store);

  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [selectedUsers, setSelectedUsers] = useState([]);
  const persistentSearchQuery = useRef('');

  let debounceResult = null;

  const getSearchResults = useCallback(() => {
    searchUsers({ state }, persistentSearchQuery.current).then((results) => setSearchResults(results));
  }, []);

  const debouncedSearch = useCallback(debounce(getSearchResults, 1000), []);

  const onChangeSearch = (query) => {
    setSearchQuery(query);
    persistentSearchQuery.current = query;
    debounceResult = debouncedSearch();
  };

  const invite = (user) => () => {
    if (selectedUsers.find((u) => u.id === user.id)) return;
    setSelectedUsers([...selectedUsers, user]);
  };

  return (
    <Surface style={styles.container}>
      <SelectedUsers users={selectedUsers} />
      <Searchbar
        style={styles.searchbar}
        placeholder="Search"
        onChangeText={onChangeSearch}
        value={searchQuery}
      />
      <ScrollView>
        {searchResults && searchResults.map((result) => (
          <UserCard
            user={result}
            actions={(
              <Button onPress={invite(result)}>
                Invite
              </Button>
            )}
          />
        ))}
      </ScrollView>
      <FAB
        style={styles.fab}
        icon="send"
        label="Invite"
        disabled={!selectedUsers || selectedUsers.length === 0}
        onPress={() => {
          selectedUsers.forEach((user) => {
            sendInvitation({ state }, user.id, state.group.id);
          });
          navigation.goBack();
        }}
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
  },
  searchResult: {
    margin: 5
  },
  selectedUserContainer: {
    margin: 5
  },
  fab: {
    margin: 10, right: 0, position: 'absolute', bottom: 0
  }
});

export default Invite;
