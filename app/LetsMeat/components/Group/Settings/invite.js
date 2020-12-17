import React, {
  useCallback, useContext, useState, useRef
} from 'react';
import { debounce } from 'debounce';
import {
  StyleSheet, Text, View, Image
} from 'react-native';
import { ScrollView } from 'react-native-gesture-handler';
import {
  Card, Chip, Searchbar, Surface, Button, Avatar, FAB
} from 'react-native-paper';
import { searchUsers, sendInvite } from '../../Requests';
import { store } from '../../Store';

const SelectedUsers = ({ users, onClose }) =>
// placeholder
{
  if (users && users.length > 0) console.log(users[0]);
  return (
    <View style={styles.selectedUserContainer}>
      <ScrollView horizontal>
        {users ? users.map((user) => (
          <Chip
            key={user.id}
            // TODO: learn how to add ana avatar
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
};

const Invite = ({ navigation }) => {
  const { state, dispatch } = useContext(store);

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
      <SelectedUsers users={selectedUsers} onClose={console.log} />
      <Searchbar
        style={styles.searchbar}
        placeholder="Search"
        onChangeText={onChangeSearch}
        value={searchQuery}
      />
      <ScrollView>
        {searchResults && searchResults.map((result) => (
          <Card key={result.id} style={styles.searchResult}>
            <Card.Title title={result.name} />
            <Card.Content>
              <Avatar.Image source={{ uri: result.picture_url }} />
            </Card.Content>
            <Card.Actions>
              <Button onPress={invite(result)}>
                Invite
              </Button>
            </Card.Actions>
          </Card>
        ))}
      </ScrollView>
      <FAB
        style={styles.fab}
        icon="send"
        label="Invite"
        disabled={!selectedUsers || selectedUsers.length === 0}
        onPress={() => {
          selectedUsers.forEach((user) => {
            sendInvite({ state }, user.id, state.group.id);
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
