import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import {
  Image, StyleSheet, View
} from 'react-native';
import ImagePicker from 'react-native-image-crop-picker';
import {
  ActivityIndicator, Button, Caption, Card, Paragraph
} from 'react-native-paper';
import {
  getImagesInfo, uploadImage, deleteImage, deleteImageDebt, createImageDebt, addDebt
} from '../../Requests';
import { store } from '../../Store';
import { UserPicker } from '../../User';

const Debt = ({
  debt, owner = false, assignedTo, onAssign, onClaim, onEdit, navigation, imageId
}) => {
  const { state } = useContext(store);
  const [visible, setVisible] = useState(true);
  const [userPickerOpen, setUserPickerOpen] = useState(false);
  const [user, setUser] = useState(assignedTo || null);

  console.log(debt);

  return (
    visible ? (
      <>
        <Card style={{ margin: 5 }}>
          <Card.Title title={debt.amount / 100} />
          <Card.Content>
            <Caption>
              {debt.satisfied
                ? (
                  `This debt has been accepted by
                ${user.name}`
                )
                : (user
                  ? `This debt has been assigned to ${user.name}`
                  : 'No one has claimed this debt yet')}
            </Caption>
            <Paragraph>
              {debt.description}
            </Paragraph>
          </Card.Content>
          <Card.Actions style={{ justifyContent: 'space-evenly' }}>
            {!owner && (
            <Button
              onPress={onClaim}
            >
              Claim
            </Button>
            )}
            {owner && (
            <>
              <Button
                onPress={() => setUserPickerOpen(true)}
              >
                Assign
              </Button>
              <Button
                onPress={() => {
                  navigation.navigate('AddDebt', { imageId, debt });
                }}
              >
                Edit
              </Button>
              <Button
                color="red"
                onPress={() => {
                  deleteImageDebt({ state }, debt.id).then(setVisible(false));
                }}
              >
                Delete
              </Button>
            </>
            )}
          </Card.Actions>
        </Card>
        <UserPicker
          userIds={state.group.users.map((u) => u.id).filter((id) => id !== state.user.id)}
          dialogVisible={userPickerOpen}
          onDismiss={() => setUserPickerOpen(false)}
          setUser={(newUser) => {
            console.log('user');
            console.log(newUser);
            addDebt({ state }, state.group.id, state.event.id, newUser.id, state.user.id, null, null, debt.id, 0)
              .then(() => setUser(newUser));
          }}
        />
      </>
    ) : null);
};

const DebtImage = ({ id, navigation }) => {
  const { state, dispatch } = useContext(store);
  const [imageData, setImageData] = useState(null);
  const [imageSize, setImageSize] = useState({ width: 0, height: 0 });

  useEffect(() => {
    getImagesInfo({ state }, id).then((info) => {
      Image.getSize(info[0].image_url, (width, height) => {
        setImageSize({ width, height });
        setImageData(info[0]);
      }).catch(console.log);
    });
  }, [id, state]);

  return (
    imageData
      ? (
        <Card style={{ margin: 5 }}>
          <Card.Content>
            <Image
              style={{ width: '100%', height: undefined, aspectRatio: imageSize.width / imageSize.height || 0 }}
              source={{ uri: imageData.image_url }}
            />
          </Card.Content>
          <Card.Content>
            {(imageData.debts_from_image || [])
              .map((d) => <Debt debt={d} key={d.id} owner={state.user.id === imageData.uploaded_by} navigation={navigation} imageId={id} />)}
          </Card.Content>
          <Card.Actions style={{ justifyContent: 'space-evenly' }}>
            <Button onPress={
              () => {
                navigation.navigate('AddDebt', { eventId: state.event.id, imageId: id });
              }
            }
            >
              <Icon name="plus" size={25} />
            </Button>
            <Button onPress={
              () => {
                deleteImage({ state }, id).then(() => {
                  dispatch({ type: 'REMOVE_IMAGE', imageId: id });
                });
              }
            }
            >
              <Icon name="delete" size={25} />
            </Button>
          </Card.Actions>
        </Card>
      )
      : <ActivityIndicator />
  );
};

const Images = ({ images, navigation }) => (images
  ? images.map((id) => (
    <DebtImage
      navigation={navigation}
      key={id}
      id={id}
    />
  ))
  : <></>);

const Debts = ({ images, onAdd, navigation }) => {
  const { state, dispatch } = useContext(store);
  const [loading, setLoading] = useState(true);

  return (
    <Card style={styles.section} elevation={0}>
      <Card.Title title="Debts" />
      <Images
        navigation={navigation}
        images={images}
      />
      <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
        <Button
          style={styles.addButton}
          onPress={
          () => {
            ImagePicker.openPicker({
              cropping: true
            })
              .then((i) => uploadImage({ state }, state.event.id, i))
              .then((data) => {
                dispatch({ type: 'ADD_IMAGE_TO_EVENT', imageId: data.image_id });
              })
              .catch((e) => {
                if (e.code === 'E_PICKER_CANCELLED') return;
                throw e;
              });
          }
        }
        >
          <Icon name="image-plus" size={25} />
        </Button>
      </View>
    </Card>
  );
};

const styles = StyleSheet.create({
  eventTitle: {
    fontSize: 30,
    marginHorizontal: 20,
    marginTop: 20
  },
  container: {
    width: '100%',
    height: '100%'
  },
  section: {
    margin: 10
  },
  card: {
    margin: 25
  },
  addButton: {
    marginBottom: 10
  },
  timeCard: {
    margin: 5,
    justifyContent: 'center'
  }
});

export default Debts;
