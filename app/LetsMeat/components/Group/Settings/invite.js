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
import { set } from 'react-native-reanimated';
import { searchUsers, sendInvitation } from '../../Requests';
import { store } from '../../Store';
import UserCard from '../../User';
import BackgroundContainer from '../../Background';

const SelectedUsers = ({ users, onClose }) => (
  <View style={styles.selectedUserContainer}>
    <ScrollView horizontal>
      {users ? users.map((user) => (
        <Chip
          key={user.id}
          avatar={<Avatar.Image source={{ uri: user.picture_url }} size={24} />}
          onClose={() => onClose(user.id)}
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

const Invite = ({ navigation, route }) => {
  const _mounted = useRef(false);

  const { state, dispatch } = useContext(store);

  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [selectedUsers, setSelectedUsers] = useState([]);
  const persistentSearchQuery = useRef('');

  let debounceResult = null;

  const getSearchResults = useCallback(() => {
    searchUsers({ state, dispatch }, persistentSearchQuery.current).then((results) => {
      if (!_mounted.current) return;
      setSearchResults(results);
    });
  }, [state]);

  const debouncedSearch = useCallback(debounce(getSearchResults, 1000), []);

  const onChangeSearch = (query) => {
    setSearchQuery(query);
    persistentSearchQuery.current = query;
    if (query.length <= 3) return;
    debounceResult = debouncedSearch();
  };

  const invite = (user) => () => {
    if (selectedUsers.find((u) => u.id === user.id)) return;
    console.log(user);
    setSelectedUsers([...selectedUsers, user]);
  };

  useEffect(() => {
    _mounted.current = true;
    return () => { _mounted.current = false; };
  }, []);

  return (
    <BackgroundContainer backgroundVariant="searching">
      <SelectedUsers users={selectedUsers} onClose={(id) => { setSelectedUsers(selectedUsers.filter((u) => u.id !== id)); }} />
      <Searchbar
        style={styles.searchbar}
        placeholder="Search"
        onChangeText={onChangeSearch}
        value={searchQuery}
      />
      <ScrollView>
        {searchResults && searchResults.map((result) => {
          let disabled = false;
          let message = 'Invite';
          if (state.group.users.find((u) => u.id === result.id)) {
            disabled = true;
            message = 'Already a member';
          }
          if (selectedUsers.find((u) => u.id === result.id)) {
            disabled = true;
            message = 'Will be invited';
          }
          return (
            <UserCard
              key={result.id}
              user={result}
              actions={(
                <Button
                  disabled={disabled}
                  onPress={invite(result)}
                >
                  {message}
                </Button>
            )}
            />
          );
        })}
      </ScrollView>
      <FAB
        style={styles.fab}
        icon="send"
        label="Invite"
        disabled={!selectedUsers || selectedUsers.length === 0}
        onPress={() => {
          selectedUsers.forEach((user) => {
            sendInvitation({ state, dispatch }, user.id, state.group.id);
          });
          navigation.goBack();
        }}
      />
    </BackgroundContainer>
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
    margin: 20,
    marginBottom: 80,
    right: 0,
    position: 'absolute',
    bottom: 0,
  }
});

export default Invite;
